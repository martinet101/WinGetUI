# <img src="https://raw.githubusercontent.com/marticliment/UniGetUI/main/media/icon.png" height="40">UniGetUI (formerly WingetUI)

[![Downloads@latest](https://img.shields.io/github/downloads/marticliment/UniGetUI/3.1.1/total?style=for-the-badge)](https://github.com/marticliment/UniGetUI/releases/latest/download/UniGetUI.Installer.exe)
[![Release Version Badge](https://img.shields.io/github/v/release/marticliment/UniGetUI?style=for-the-badge)](https://github.com/marticliment/UniGetUI/releases)
[![Issues Badge](https://img.shields.io/github/issues/marticliment/UniGetUI?style=for-the-badge)](https://github.com/marticliment/UniGetUI/issues)
[![Closed Issues Badge](https://img.shields.io/github/issues-closed/marticliment/UniGetUI?color=%238256d0&style=for-the-badge)](https://github.com/marticliment/UniGetUI/issues?q=is%3Aissue+is%3Aclosed)<br>
The main goal of this project is to create an intuitive GUI for the most common CLI package managers for Windows 10 and 11, such as [WinGet](https://learn.microsoft.com/en-us/windows/package-manager/), [Scoop](https://scoop.sh/), [Chocolatey](https://chocolatey.org/), [Pip](https://pypi.org/), [Npm](https://www.npmjs.com/), [.NET Tool](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-tool-install), [PowerShell Gallery](https://www.powershellgallery.com/) and more (Check out the package manager compatibility table)!.
With this app, you can easily download, install, update, and uninstall any software published on the supported package managers — and much more!

Check out the [Supported Package Managers Table](#supported-package-managers) for more details!

**Disclaimer:** This project has no connection with any supported package managers — it's completely unofficial. Be aware that I, the developer of UniGetUI, am NOT responsible for the downloaded software. Proceed with caution

![Endpoint Badge](https://img.shields.io/endpoint?url=https%3A%2F%2Fmarticliment.com%2Fresources%2Fbadges%2Fdev-status.json)
![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/marticliment/WingetUI/dotnet-test.yml?branch=main&style=for-the-badge&label=Tests)<br>


> [!CAUTION]
> **The OFFICIAL website for UniGetUI is [https://www.marticliment.com/unigetui/](https://www.marticliment.com/unigetui/)**<br>
> **Any other website should be considered unofficial, despite what they may say.**



## Support the developer

It really does make a big difference, and is very much appreciated. Thanks :)<br>
[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/P5P86KKPB)

## Table of contents
 - **[UniGetUI Homepage](https://www.marticliment.com/unigetui/)**
 - [Table of contents](#table-of-contents)
 - [Installation](#installation)
 - [Update UniGetUI](#update-UniGetUI)
 - [Support the developer](#support-the-developer)
 - [Features](#features)
   - [Supported Package Managers](#supported-package-managers)
 - [Translating UniGetUI](#translating-UniGetUI-to-other-languages)
   - [Currently supported languages](#currently-supported-languages)
 - [Contributors](#contributors)
 - [Screenshots](#screenshots)
 - [Frequently Asked Questions](#frequently-asked-questions)
 - [Command-line Arguments](https://github.com/marticliment/UniGetUI/blob/main/cli-arguments.md)


<a href="https://hellogithub.com/repository/46bec642537f449a857215e39a1d64ae" target="_blank"><img src="https://abroad.hellogithub.com/v1/widgets/recommend.svg?rid=46bec642537f449a857215e39a1d64ae&claim_uid=u6sFoX4hC2HztbD&theme=small" alt="Featured｜HelloGitHub" /></a>

## Installation
<p>There are multiple ways to install UniGetUI — choose whichever one you prefer!</p>
 
### Microsoft Store installation (recommended)
<a href="https://apps.microsoft.com/detail/xpfftq032ptphf"><img alt="alt_text" width="240px" src="https://get.microsoft.com/images/en-us%20dark.svg" /></a> 

 
### Download UniGetUI installer:
![GitHub Release](https://img.shields.io/github/v/release/marticliment/UniGetUI?style=for-the-badge)
<p align="left"><b><a href="https://github.com/marticliment/UniGetUI/releases/latest/download/WingetUI.Installer.exe">Click here to download UniGetUI</a></b></p>


### Install UniGetUI via WinGet:
![WinGet Package Version](https://img.shields.io/winget/v/MartiCliment.UniGetUI?style=for-the-badge)
```cmd
winget install --exact --id MartiCliment.UniGetUI --source winget
```


### Install UniGetUI via Scoop:
![Scoop version](https://img.shields.io/scoop/v/unigetui?bucket=extras&style=for-the-badge)
```cmd
scoop bucket add extras
scoop install extras/unigetui
```

### Install UniGetUI via Chocolatey:
![Chocolatey Version](https://img.shields.io/chocolatey/v/wingetui?style=for-the-badge)
```cmd
choco install wingetui
```


## Update UniGetUI

UniGetUI has a built-in autoupdater. However, it can also be updated like any other package within UniGetUI (since UniGetUI is available from WinGet, Scoop and Chocolatey).


## Features

 - Install, update, and remove software from your system easily at one click: UniGetUI combines the packages from the most used package managers for windows: Winget, Chocolatey, Scoop, Pip, Npm and .NET Tool.
 - Discover new packages and filter them to easily find the package you want.
 - View detailed metadata about any package before installing it. Get the direct download URL or the name of the publisher, as well as the size of the download.
 - Easily bulk-install, update, or uninstall multiple packages at once selecting multiple packages before performing an operation
 - Automatically update packages, or be notified when updates become available. Skip versions or completely ignore updates on a per-package basis.
 - Manage your available updates at the touch of a button from the **Widgets pane** or from **Dev Home** pane with [Widgets for UniGetUI](https://apps.microsoft.com/detail/9NB9M5KZ8SLX)*.
 - The system tray icon will also show the available updates and installed packages, to efficiently update a program or remove a package from your system.
 - Easily customize how and where packages are installed. Select different installation options and switches for each package. Install an older version or force to install a 32 bit architecture. \[But don't worry, those options will be saved for future updates for this package*]
 - Share packages with your friends to show them off that program you found. Here is an example: [Hey \@friend, Check out this program!](https://marticliment.com/unigetui/share/?pname=Google%20Chrome&pid=Google.Chrome&psource=Winget:%20winget)
 - Export custom lists of packages to then import them to another machine and install those packages with previously specified, custom installation parameters. Setting up machines or configuring a specific software setup has never been easier.
 - Backup your packages to a local file to easily recover your setup in a matter of seconds when migrating to a new machine*

## Supported Package Managers

**NOTE:** All package managers do support basic install, update, and uninstall processes, as well as checking for updates, finding new packages, and retrieving details from a package.

<img src="https://marticliment.com/unigetui/extra/supported-managers.svg"/>

✅: Supported on UniGetUI<br>
☑️: Not directly supported but can be easily achieved<br>
⚠️: Some packages might not follow this setting<br>
❌: Not supported by the Package Manager<br>
<br>
**1\.** Some packages do not support installing to a custom location or scope and will ignore this setting<br>
**2\.** Despite the Package Manager may not support _PreReleases_, some packages can be found duplicated, with one of the copies being the beta version of it.<br>
**3\.** Some installers do not have a GUI, and will ignore the `interactive` flag<br>

# Translating UniGetUI to other languages
To translate UniGetUI to other languages or to update an old translation, please see [Translating UniGetUI - UniGetUI Wiki](https://github.com/marticliment/UniGetUI/wiki#translating-wingetui) for more info.


## Currently Supported languages
<!-- Autogenerated translations -->
| Language | Translated | Translator(s) |
| :-- | :-- | --- |
| <img src='https://flagcdn.com/sa.svg' width=20> &nbsp; Arabic - عربي‎ | 99% | [Abdu11ahAS](https://github.com/Abdu11ahAS), [Abdullah-Dev115](https://github.com/Abdullah-Dev115), [bassuny3003](https://github.com/bassuny3003), [DaRandomCube](https://github.com/DaRandomCube), [FancyCookin](https://github.com/FancyCookin), [mo9a7i](https://github.com/mo9a7i) |
| <img src='https://flagcdn.com/bg.svg' width=20> &nbsp; Bulgarian - български | 52% | Vasil Kolev |
| <img src='https://flagcdn.com/bd.svg' width=20> &nbsp; Bangla - বাংলা | 80% | [fluentmoheshwar](https://github.com/fluentmoheshwar), [itz-rj-here](https://github.com/itz-rj-here), Mushfiq Iqbal Rayon, Nilavra Bhattacharya, [samiulislamsharan](https://github.com/samiulislamsharan) |
| <img src='https://flagcdn.com/ad.svg' width=20> &nbsp; Catalan - Català | 100% | [marticliment](https://github.com/marticliment) |
| <img src='https://flagcdn.com/cz.svg' width=20> &nbsp; Czech - Čeština | 100% | [mlisko](https://github.com/mlisko), [panther7](https://github.com/panther7), [xtorlukas](https://github.com/xtorlukas) |
| <img src='https://flagcdn.com/dk.svg' width=20> &nbsp; Danish - Dansk | 65% | [AAUCrisp](https://github.com/AAUCrisp), [mikkolukas](https://github.com/mikkolukas), [yrjarv](https://github.com/yrjarv) |
| <img src='https://flagcdn.com/de.svg' width=20> &nbsp; German - Deutsch | 100% | [1270o1](https://github.com/1270o1), [alxhu-dev](https://github.com/alxhu-dev), [Araxxas](https://github.com/Araxxas), [CanePlayz](https://github.com/CanePlayz), [Datacra5H](https://github.com/Datacra5H), [ebnater](https://github.com/ebnater), [martinwilco](https://github.com/martinwilco), [michaelmairegger](https://github.com/michaelmairegger), [Seeloewen](https://github.com/Seeloewen), [yrjarv](https://github.com/yrjarv) |
| <img src='https://flagcdn.com/gr.svg' width=20> &nbsp; Greek - Ελληνικά | 89% | [antwnhsx](https://github.com/antwnhsx), [seijind](https://github.com/seijind), [thunderstrike116](https://github.com/thunderstrike116), [wobblerrrgg](https://github.com/wobblerrrgg) |
| <img src='https://flagcdn.com/ee.svg' width=20> &nbsp; Estonian - Eesti | 55% | [artjom3729](https://github.com/artjom3729) |
| <img src='https://flagcdn.com/gb.svg' width=20> &nbsp; English - English | 100% | [marticliment](https://github.com/marticliment), [ppvnf](https://github.com/ppvnf) |
| <img src='https://flagcdn.com/es.svg' width=20> &nbsp; Spanish - Castellano | 99% | [apazga](https://github.com/apazga), [dalbitresb12](https://github.com/dalbitresb12), [evaneliasyoung](https://github.com/evaneliasyoung), [guplem](https://github.com/guplem), [JMoreno97](https://github.com/JMoreno97), [marticliment](https://github.com/marticliment), [P10Designs](https://github.com/P10Designs), [rubnium](https://github.com/rubnium), [uKER](https://github.com/uKER) |
| <img src='https://flagcdn.com/ir.svg' width=20> &nbsp; Persian - فارسی‎ | 76% | [Imorate](https://github.com/Imorate), [itsarian](https://github.com/itsarian), [Mahdi-Hazrati](https://github.com/Mahdi-Hazrati), [moon24-s](https://github.com/moon24-s), [saeed205](https://github.com/saeed205), [smsi2001](https://github.com/smsi2001) |
| <img src='https://flagcdn.com/fi.svg' width=20> &nbsp; Finnish - Suomi | 96% | [simakuutio](https://github.com/simakuutio) |
| <img src='https://flagcdn.com/fr.svg' width=20> &nbsp; French - Français | 100% | BreatFR, Evans Costa, [PikPakPik](https://github.com/PikPakPik), Rémi Guerrero, [W1L7dev](https://github.com/W1L7dev) |
| <img src='https://flagcdn.com/in.svg' width=20> &nbsp; Gujarati - ગુજરાતી | 9% |  |
| <img src='https://flagcdn.com/in.svg' width=20> &nbsp; Hindi - हिंदी | 47% | [atharva_xoxo](https://github.com/atharva_xoxo), [satanarious](https://github.com/satanarious) |
| <img src='https://flagcdn.com/hr.svg' width=20> &nbsp; Croatian - Hrvatski | 53% | Stjepan Treger |
| <img src='https://flagcdn.com/il.svg' width=20> &nbsp; Hebrew - עִבְרִית‎ | 89% | [maximunited](https://github.com/maximunited), Oryan Hassidim |
| <img src='https://flagcdn.com/hu.svg' width=20> &nbsp; Hungarian - Magyar | 100% | [gidano](https://github.com/gidano) |
| <img src='https://flagcdn.com/it.svg' width=20> &nbsp; Italian - Italiano | 100% | David Senoner, [giacobot](https://github.com/giacobot), [maicol07](https://github.com/maicol07), [mapi68](https://github.com/mapi68), [mrfranza](https://github.com/mrfranza), Rosario Di Mauro |
| <img src='https://flagcdn.com/id.svg' width=20> &nbsp; Indonesian - Bahasa Indonesia | 72% | [arthackrc](https://github.com/arthackrc), [joenior](https://github.com/joenior) |
| <img src='https://flagcdn.com/jp.svg' width=20> &nbsp; Japanese - 日本語 | 89% | [anmoti](https://github.com/anmoti), [nob-swik](https://github.com/nob-swik), Nobuhiro Shintaku, sho9029, [tacostea](https://github.com/tacostea), Yuki Takase |
| <img src='https://flagcdn.com/in.svg' width=20> &nbsp; Kannada - ಕನ್ನಡ | 12% | [skanda890](https://github.com/skanda890) |
| <img src='https://flagcdn.com/kr.svg' width=20> &nbsp; Korean - 한국어 | 73% | [minbert](https://github.com/minbert), [shblue21](https://github.com/shblue21), [VenusGirl](https://github.com/VenusGirl) |
| <img src='https://flagcdn.com/lt.svg' width=20> &nbsp; Lithuanian - Lietuvių | 99% | [dziugas1959](https://github.com/dziugas1959), Džiugas Januševičius, [martyn3z](https://github.com/martyn3z) |
| <img src='https://flagcdn.com/mk.svg' width=20> &nbsp; Macedonian - Македонски | 55% | LordDeatHunter |
| <img src='https://flagcdn.com/no.svg' width=20> &nbsp; Norwegian (bokmål) | 89% | [yrjarv](https://github.com/yrjarv) |
| <img src='https://flagcdn.com/no.svg' width=20> &nbsp; Norwegian (nynorsk) | 88% | [yrjarv](https://github.com/yrjarv) |
| <img src='https://flagcdn.com/nl.svg' width=20> &nbsp; Dutch - Nederlands | 100% | [abbydiode](https://github.com/abbydiode), [CateyeNL](https://github.com/CateyeNL), [Stephan-P](https://github.com/Stephan-P) |
| <img src='https://flagcdn.com/pl.svg' width=20> &nbsp; Polish - Polski | 95% | [GrzegorzKi](https://github.com/GrzegorzKi), [KamilZielinski](https://github.com/KamilZielinski), [kwiateusz](https://github.com/kwiateusz), [RegularGvy13](https://github.com/RegularGvy13), [ThePhaseless](https://github.com/ThePhaseless) |
| <img src='https://flagcdn.com/br.svg' width=20> &nbsp; Portuguese (Brazil) | 100% | [maisondasilva](https://github.com/maisondasilva), [ppvnf](https://github.com/ppvnf), [Rodrigo-Matsuura](https://github.com/Rodrigo-Matsuura), [thiagojramos](https://github.com/thiagojramos), [wanderleihuttel](https://github.com/wanderleihuttel) |
| <img src='https://flagcdn.com/pt.svg' width=20> &nbsp; Portuguese (Portugal) | 100% | [PoetaGA](https://github.com/PoetaGA), [Tiago_Ferreira](https://github.com/Tiago_Ferreira) |
| <img src='https://flagcdn.com/ro.svg' width=20> &nbsp; Romanian - Română | 100% | [SilverGreen93](https://github.com/SilverGreen93), TZACANEL |
| <img src='https://flagcdn.com/ru.svg' width=20> &nbsp; Russian - Русский | 100% | [bropines](https://github.com/bropines), [Denisskas](https://github.com/Denisskas), [DvladikD](https://github.com/DvladikD), [flatron4eg](https://github.com/flatron4eg), Gleb Saygin, [katrovsky](https://github.com/katrovsky), Sergey, sklart |
| <img src='https://flagcdn.com/in.svg' width=20> &nbsp; Sanskrit - संस्कृत भाषा | 11% | [skanda890](https://github.com/skanda890) |
| <img src='https://flagcdn.com/sk.svg' width=20> &nbsp; Slovak - Slovenčina | 11% | [Luk164](https://github.com/Luk164) |
| <img src='https://flagcdn.com/rs.svg' width=20> &nbsp; Serbian - Srpski | 88% | [daVinci13](https://github.com/daVinci13), [momcilovicluka](https://github.com/momcilovicluka) |
| <img src='https://flagcdn.com/al.svg' width=20> &nbsp; Albanian - Shqip | 100% | [RDN000](https://github.com/RDN000) |
| <img src='https://flagcdn.com/lk.svg' width=20> &nbsp; Sinhala - සිංහල | 7% | [SashikaSandeepa](https://github.com/SashikaSandeepa), [ttheek](https://github.com/ttheek) |
| <img src='https://flagcdn.com/si.svg' width=20> &nbsp; Slovene - Slovenščina | 97% | [rumplin](https://github.com/rumplin) |
| <img src='https://flagcdn.com/se.svg' width=20> &nbsp; Swedish - Svenska | 43% | [curudel](https://github.com/curudel) |
| <img src='https://flagcdn.com/ph.svg' width=20> &nbsp; Tagalog - Tagalog | 12% | lasersPew |
| <img src='https://flagcdn.com/th.svg' width=20> &nbsp; Thai - ภาษาไทย | 86% | [apaeisara](https://github.com/apaeisara), [dulapahv](https://github.com/dulapahv), [rikoprushka](https://github.com/rikoprushka) |
| <img src='https://flagcdn.com/tr.svg' width=20> &nbsp; Turkish - Türkçe | 100% | [ahmetozmtn](https://github.com/ahmetozmtn), [dogancanyr](https://github.com/dogancanyr), [gokberkgs](https://github.com/gokberkgs) |
| <img src='https://flagcdn.com/ua.svg' width=20> &nbsp; Ukrainian - Українська | 100% | Artem Moldovanenko, Operator404, [Taron-art](https://github.com/Taron-art) |
| <img src='https://flagcdn.com/pk.svg' width=20> &nbsp; Urdu - اردو | 62% | [digitio](https://github.com/digitio), [digitpk](https://github.com/digitpk) |
| <img src='https://flagcdn.com/vn.svg' width=20> &nbsp; Vietnamese - Tiếng Việt | 94% | [legendsjoon](https://github.com/legendsjoon), [long5436](https://github.com/long5436), [txavlog](https://github.com/txavlog) |
| <img src='https://flagcdn.com/cn.svg' width=20> &nbsp; Simplified Chinese (China) | 100% | Aaron Liu, adfnekc, [Ardenet](https://github.com/Ardenet), [arthurfsy2](https://github.com/arthurfsy2), [bai0012](https://github.com/bai0012), BUGP Association, ciaran, CnYeSheng, Cololi, [dongfengweixiao](https://github.com/dongfengweixiao), [enKl03B](https://github.com/enKl03B), [seanyu0](https://github.com/seanyu0), [Sigechaishijie](https://github.com/Sigechaishijie), [SpaceTimee](https://github.com/SpaceTimee), Yisme |
| <img src='https://flagcdn.com/tw.svg' width=20> &nbsp; Traditional Chinese (Taiwan) | 100% | Aaron Liu, CnYeSheng, Cololi, [enKl03B](https://github.com/enKl03B), [Henryliu880922](https://github.com/Henryliu880922), [StarsShine11904](https://github.com/StarsShine11904), [yrctw](https://github.com/yrctw) |

Last updated: Mon Feb  3 00:13:38 2025
<!-- END Autogenerated translations -->


# Contributions
 UniGetUI wouldn't have been possible without the help of our dear contributors. From the person who fixed a typo to the person who improved half of the code, UniGetUI wouldn't be possible without them! :smile:<br><br>

## Contributors:
 [![My dear contributors](https://contrib.rocks/image?repo=marticliment/UniGetUI)](https://github.com/marticliment/UniGetUI/graphs/contributors)<br><br>
 

# Screenshots
 
![image](https://raw.githubusercontent.com/marticliment/UniGetUI/main/media/UniGetUI_1.png)

![image](https://raw.githubusercontent.com/marticliment/UniGetUI/main/media/UniGetUI_2.png)

![image](https://raw.githubusercontent.com/marticliment/UniGetUI/main/media/UniGetUI_3.png)

![image](https://raw.githubusercontent.com/marticliment/UniGetUI/main/media/UniGetUI_4.png)

![image](https://raw.githubusercontent.com/marticliment/UniGetUI/main/media/UniGetUI_5.png)

![image](https://raw.githubusercontent.com/marticliment/UniGetUI/main/media/UniGetUI_6.png)

![image](https://raw.githubusercontent.com/marticliment/UniGetUI/main/media/UniGetUI_7.png)

![image](https://raw.githubusercontent.com/marticliment/UniGetUI/main/media/UniGetUI_8.png)

![image](https://raw.githubusercontent.com/marticliment/UniGetUI/main/media/UniGetUI_9.png)

![image](https://raw.githubusercontent.com/marticliment/UniGetUI/main/media/UniGetUI_10.png)


# Frequently asked questions

**Q: I am unable to install or upgrade a specific Winget package! What should I do?**<br>

A: This is likely an issue with Winget rather than UniGetUI. 

Please check if it's possible to install/upgrade the package through PowerShell or the Command Prompt by using the commands `winget upgrade` or `winget install`, depending on the situation (for example: `winget upgrade --id Microsoft.PowerToys`). 

If this doesn't work, consider asking for help at [Winget's project page](https://github.com/microsoft/winget-cli).<br>

#

**Q: The name of a package is trimmed with ellipsis — how do I see its full name/id?**<br>

A: This is a known limitation of Winget. 

For more details, see this issue: https://github.com/microsoft/winget-cli/issues/2603.<br>

#

**Q: My antivirus is telling me that UniGetUI is a virus! / My browser is blocking the download of UniGetUI!**<br>

A: A common reason apps (i.e., executables) get blocked and/or detected as a virus — even when there's nothing malicious about them, like in the case of UniGetUI — is because a relatively large amount of people are not using them.

Combine that with the fact that you might be downloading something recently released, and blocking unknown apps is in many cases a good precaution to take to prevent actual malware.

Since UniGetUI is open source and safe to use, whitelist the app in the settings of your antivirus/browser.<br>

#

**Q: Are Winget/Scoop packages safe?**<br>

A: UniGetUI, Microsoft, and Scoop aren't responsible for the packages available for download, which are provided by third parties and can theoretically be compromised.

Microsoft has implemented a few checks for the software available on Winget to mitigate the risks of downloading malware. Even so, it's recommended that you only download software from trusted publishers. 

<br><p align="center"><i>Check out the <a href="https://github.com/marticliment/UniGetUI/wiki">Wiki</a> for more information!</i></p>

## Command-line parameters:

Check out the full list of parameters [here](https://github.com/marticliment/UniGetUI/blob/main/cli-arguments.md)
