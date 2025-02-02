using UniGetUI.Core.Logging;
using UniGetUI.Core.SettingsEngine;
using UniGetUI.Core.Tools;
using UniGetUI.PackageEngine.Enums;

namespace UniGetUI.PackageOperations;

public abstract class AbstractOperation : IDisposable
{
    public static class RetryMode
    {
        public const string NoRetry = "";
        public const string Retry = "Retry";
        public const string Retry_AsAdmin = "RetryAsAdmin";
        public const string Retry_Interactive = "RetryInteractive";
        public const string Retry_SkipIntegrity = "RetryNoHashCheck";
    }

    public class OperationMetadata
    {
        /// <summary>
        /// Installation of X
        /// </summary>
        public string Title = "";

        /// <summary>
        /// X is being installed/upated/removed
        /// </summary>
        public string Status = "";

        /// <summary>
        /// X was installed
        /// </summary>
        public string SuccessTitle = "";

        /// <summary>
        /// X has been installed successfully
        /// </summary>
        public string SuccessMessage = "";

        /// <summary>
        /// X could not be installed.
        /// </summary>
        public string FailureTitle = "";

        /// <summary>
        /// X Could not be installed
        /// </summary>
        public string FailureMessage = "";

        /// <summary>
        /// Starting operation X with options Y
        /// </summary>
        public string OperationInformation = "";

        public readonly string Identifier;

        public OperationMetadata()
        {
            Identifier  =  new Random().NextInt64(1000000, 9999999).ToString();
        }
    }

    public readonly OperationMetadata Metadata = new();
    public static readonly List<AbstractOperation> OperationQueue = new();

    public event EventHandler<OperationStatus>? StatusChanged;
    public event EventHandler<EventArgs>? CancelRequested;
    public event EventHandler<(string, LineType)>? LogLineAdded;
    public event EventHandler<EventArgs>? OperationStarting;
    public event EventHandler<EventArgs>? OperationFinished;
    public event EventHandler<EventArgs>? Enqueued;
    public event EventHandler<EventArgs>? OperationSucceeded;
    public event EventHandler<EventArgs>? OperationFailed;

    public static int MAX_OPERATIONS;

    public event EventHandler<BadgeCollection>? BadgesChanged;

    public class BadgeCollection
    {
        public readonly bool AsAdministrator;
        public readonly bool Interactive;
        public readonly bool SkipHashCheck;
        public readonly PackageScope? Scope;

        public BadgeCollection(bool admin, bool interactive, bool skiphash, PackageScope? scope)
        {
            AsAdministrator = admin;
            Interactive = interactive;
            SkipHashCheck = skiphash;
            Scope = scope;
        }
    }
    public void ApplyCapabilities(bool admin, bool interactive, bool skiphash, PackageScope? scope)
    {
        BadgesChanged?.Invoke(this, new BadgeCollection(admin, interactive, skiphash, scope));
    }

    public enum LineType
    {
        VerboseDetails,
        ProgressIndicator,
        Information,
        Error
    }

    private List<(string, LineType)> LogList = new();
    private OperationStatus _status = OperationStatus.InQueue;
    public OperationStatus Status
    {
        get => _status;
        set { _status = value; StatusChanged?.Invoke(this, value); }
    }

    public bool Started { get; private set; }
    protected bool QUEUE_ENABLED;
    protected bool FORCE_HOLD_QUEUE;

    private AbstractOperation? requirement;

    public AbstractOperation(bool queue_enabled, AbstractOperation? req)
    {
        QUEUE_ENABLED = queue_enabled;
        if (req is not null)
        {
            requirement = req;
            QUEUE_ENABLED = false;
        }

        Status = OperationStatus.InQueue;
        Line("Please wait...", LineType.ProgressIndicator);

        if(int.TryParse(Settings.GetValue("ParallelOperationCount"), out int _maxPps))
        {
            MAX_OPERATIONS = _maxPps;
            Logger.Debug($"Parallel operation limit set to {MAX_OPERATIONS}");
        }
        else
        {
            MAX_OPERATIONS = 1;
            Logger.Debug("Parallel operation limit not set, defaulting to 1");
        }
    }

    public void Cancel()
    {
        switch (_status)
        {
            case OperationStatus.Canceled:
                break;
            case OperationStatus.Failed:
                break;
            case OperationStatus.Running:
                Status = OperationStatus.Canceled;
                while(OperationQueue.Remove(this));
                CancelRequested?.Invoke(this, EventArgs.Empty);
                Status = OperationStatus.Canceled;
                break;
            case OperationStatus.InQueue:
                Status = OperationStatus.Canceled;
                while(OperationQueue.Remove(this));
                Status = OperationStatus.Canceled;
                break;
            case OperationStatus.Succeeded:
                break;
        }
    }

    protected void Line(string line, LineType type)
    {
        if(type != LineType.ProgressIndicator) LogList.Add((line, type));
        LogLineAdded?.Invoke(this, (line, type));
    }

    public IReadOnlyList<(string, LineType)> GetOutput()
    {
        return LogList;
    }

    public async Task MainThread()
    {
        try
        {
            if (Metadata.Status == "") throw new InvalidDataException("Metadata.Status was not set!");
            if (Metadata.Title == "") throw new InvalidDataException("Metadata.Title was not set!");
            if (Metadata.OperationInformation == "")
                throw new InvalidDataException("Metadata.OperationInformation was not set!");
            if (Metadata.SuccessTitle == "") throw new InvalidDataException("Metadata.SuccessTitle was not set!");
            if (Metadata.SuccessMessage == "") throw new InvalidDataException("Metadata.SuccessMessage was not set!");
            if (Metadata.FailureTitle == "") throw new InvalidDataException("Metadata.FailureTitle was not set!");
            if (Metadata.FailureMessage == "") throw new InvalidDataException("Metadata.FailureMessage was not set!");

            Started = true;

            if (OperationQueue.Contains(this))
                throw new InvalidOperationException("This operation was already on the queue");

            Status = OperationStatus.InQueue;
            Line(Metadata.OperationInformation, LineType.VerboseDetails);
            Line(Metadata.Status, LineType.ProgressIndicator);

            Enqueued?.Invoke(this, EventArgs.Empty);

            if (requirement != null)
            {   // OPERATION REQUIREMENT HANDLER
                Logger.Info($"Operation {Metadata.Title} is waiting for requirement operation {requirement.Metadata.Title}");
                Line(CoreTools.Translate("Waiting for {0} to complete...", requirement.Metadata.Title), LineType.ProgressIndicator);
                {
                    while (requirement.Status is OperationStatus.Running or OperationStatus.InQueue)
                    {
                        await Task.Delay(100);
                        if(SKIP_QUEUE) break;
                    }
                }
            }
            else if (QUEUE_ENABLED)
            {   // QUEUE HANDLER
                SKIP_QUEUE = false;
                OperationQueue.Add(this);
                int lastPos = -2;

                while (FORCE_HOLD_QUEUE || (OperationQueue.IndexOf(this) >= MAX_OPERATIONS && !SKIP_QUEUE))
                {
                    int pos = OperationQueue.IndexOf(this) - MAX_OPERATIONS + 1;

                    if (pos == -1) return;
                    // In this case, operation was canceled;

                    if (pos != lastPos)
                    {
                        lastPos = pos;
                        Line(CoreTools.Translate("Operation on queue (position {0})...", pos), LineType.ProgressIndicator);
                    }

                    await Task.Delay(100);
                }
            }
            // END QUEUE HANDLER

            // BEGIN ACTUAL OPERATION
            OperationVeredict result;
            Line(CoreTools.Translate("Starting operation..."), LineType.ProgressIndicator);
            if(Status is OperationStatus.InQueue) Status = OperationStatus.Running;
            OperationStarting?.Invoke(this, EventArgs.Empty);

            do
            {
                try
                {
                    // Check if the operation was canceled
                    if (Status is OperationStatus.Canceled)
                    {
                        result = OperationVeredict.Canceled;
                        break;
                    }
                    else if (requirement is not null)
                    {
                        if (requirement.Status is OperationStatus.Failed)
                        {
                            Line(CoreTools.Translate("{0} has failed, that was a requirement for {1} to be run", requirement.Metadata.Title, Metadata.Title), LineType.Error);
                            result = OperationVeredict.Failure;
                            break;
                        }
                        else if (requirement.Status is OperationStatus.Canceled)
                        {
                            Line(CoreTools.Translate("The user has canceled {0}, that was a requirement for {1} to be run", requirement.Metadata.Title, Metadata.Title), LineType.Error);
                            result = OperationVeredict.Canceled;
                            break;
                        }
                    }

                    Task<OperationVeredict> op = PerformOperation();
                    while (Status != OperationStatus.Canceled && !op.IsCompleted) await Task.Delay(100);

                    if (Status is OperationStatus.Canceled) result = OperationVeredict.Canceled;
                    else result = op.GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    result = OperationVeredict.Failure;
                    Logger.Error(e);
                    foreach (string l in e.ToString().Split("\n")) Line(l, LineType.Error);
                }
            } while (result == OperationVeredict.AutoRetry);

            OperationFinished?.Invoke(this, EventArgs.Empty);

            while (OperationQueue.Remove(this));
            // END OPERATION

            if (result == OperationVeredict.Success)
            {
                Status = OperationStatus.Succeeded;
                OperationSucceeded?.Invoke(this, EventArgs.Empty);
                Line(Metadata.SuccessMessage, LineType.Information);
            }
            else if (result == OperationVeredict.Failure)
            {
                Status = OperationStatus.Failed;
                OperationFailed?.Invoke(this, EventArgs.Empty);
                Line(Metadata.FailureMessage, LineType.Error);
                Line(Metadata.FailureMessage + " - " + CoreTools.Translate("Click here for more details"),
                    LineType.ProgressIndicator);
            }
            else if (result == OperationVeredict.Canceled)
            {
                Status = OperationStatus.Canceled;
                Line(CoreTools.Translate("Operation canceled by user"), LineType.Error);
            }
        }
        catch (Exception ex)
        {
            Line("An internal error occurred:", LineType.Error);
            foreach (var line in ex.ToString().Split("\n"))
                Line(line, LineType.Error);

            while (OperationQueue.Remove(this)) ;

            Status = OperationStatus.Failed;
            try
            {
                OperationFinished?.Invoke(this, EventArgs.Empty);
                OperationFailed?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception e2)
            {
                Line("An internal error occurred while handling an internal error:", LineType.Error);
                foreach (var line in e2.ToString().Split("\n"))
                    Line(line, LineType.Error);
            }

            Line(Metadata.FailureMessage, LineType.Error);
            Line(Metadata.FailureMessage + " - " + CoreTools.Translate("Click here for more details"),
                LineType.ProgressIndicator);
        }
    }

    private bool SKIP_QUEUE;

    public void SkipQueue()
    {
        if (Status != OperationStatus.InQueue) return;
        while(OperationQueue.Remove(this));
        SKIP_QUEUE = true;
    }

    public void RunNext()
    {
        if (Status != OperationStatus.InQueue) return;
        if (!OperationQueue.Contains(this)) return;

        FORCE_HOLD_QUEUE = true;
        while(OperationQueue.Remove(this));
        OperationQueue.Insert(Math.Min(MAX_OPERATIONS, OperationQueue.Count), this);
        FORCE_HOLD_QUEUE = false;
    }

    public void BackOfTheQueue()
    {
        if (Status != OperationStatus.InQueue) return;
        if (!OperationQueue.Contains(this)) return;

        FORCE_HOLD_QUEUE = true;
        while(OperationQueue.Remove(this));
        OperationQueue.Add(this);
        FORCE_HOLD_QUEUE = false;
    }

    public void Retry(string retryMode)
    {
        if (retryMode is RetryMode.NoRetry)
            throw new InvalidOperationException("We weren't supposed to reach this, weren't we?");

        ApplyRetryAction(retryMode);
        Line($"", LineType.VerboseDetails);
        Line($"-----------------------", LineType.VerboseDetails);
        Line($"Retrying operation with RetryMode={retryMode}", LineType.VerboseDetails);
        Line($"", LineType.VerboseDetails);
        if (Status is OperationStatus.Running or OperationStatus.InQueue) return;
        _ = MainThread();
    }

    protected abstract void ApplyRetryAction(string retryMode);
    protected abstract Task<OperationVeredict> PerformOperation();
    public abstract Task<Uri> GetOperationIcon();
    public void Dispose()
    {
        while(OperationQueue.Remove(this));
    }
}
