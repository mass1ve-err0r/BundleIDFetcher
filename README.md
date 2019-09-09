# BundleIDFetcher
Fetch App-Icons in 512x512 through iTunes' Search API

![showcaseX](/assets/showcase.png)

## Introduction
As an iOS Themer, you might have heard of offcornerdev's site to search for application icons & see their respective BundleID. However, his site *does not* allow you to search by BundleID, which makes gathering icons harder considering some BundleIDs don't "match" their app/ some apps have weird BundleIDs.

This Tool adresses just that. It allows you to search *with* the BundleID and automatically downloads its associated icon in 512x512.

It also features a Batch-Get which basically mass downloads icons from a list of BundleIDs, just provide it a list in form of a `.txt` file, more details below in *EXTRA NOTES*.

The primary focus of this is to help themers get their resources faster instead of tediously searching.

**NOTE:** If you are a themer: Please encourage your customers/ users to provide you BundleIDs instead of App-Names! That will allow you to fetch icons quicker! (Grab BundleIDs of applications with my BundleIDsXII, available on Packix [HERE](https://repo.packix.com/package/de.mass1veerr0r.bundleidsxii/))


**THIS WILL ONLY FETCH ICONS IF THE APP EXISTS ON THE APP STORE.**

System Application & external 3rd-party apps such as Emulators are excluded.


**PLEASE READ THROUGH ALL OF THE INSTALLATION STEPS & FAQ BEFORE REPORTING A BUG**


## Features
- Grab individual icons through their unique BundleID
- Batch-Get icons to quickly obtain resources. No more tedious searching
  - Shows you how many were processed & how many failed (due to bad BundleID/ Not Found)
- OK-ish UI
- Open-Soruce. There's no fancy connection being made to someplace except the iTunes Store's server to fetch the information.

*In case of doubt, look at the source code/ use wireshark/ compile it yourself.*


## Requirements
- .NET Framework 4.7.2
- Windows 10

(Tested on latest vanilla Win10 & my own PC running an older build)


## How-To Install
There's fortunately nothing you need to install, it's portable!

1. Just grab the latest release from placeholder
2. Extract anywhere, preferably onto Desktop
3. Run it & profit.

The UI should be pretty self-explanatory, else here is a quick rundown:
- Enter the BundleID in the designated Field
- Click "Get BundleID Image"
- If successfull, it'll show you a message. If an error occured, it'll let you know as well.


Downloaded icons will be stored in a folder called "\_DownlaodedImages" as `.JPG`

Folder can be found wherever BundleIDfetcher was extracted to.


## FAQ
- *It doesn't work?!*
  - Make sure you meet the requirements & make sure the BundleIDs are typed correctly
- *Batch-Get doesn't get all of the BundleID *
  - Check your BundleIDs and make sure the list is formatted correctly (see *EXTRA NOTES*)
- *Bro it STILL doesn't work*
  - Contact me and report the bug with details and your Windows Version. A screen recording would be even more helpful!


## EXTRA NOTES
This section is dedicated to the *Batch-Get* feature and how you can utelize it. This option will open a File Picker and you can only select `.txt` files.

Please note, this is the ONLY correct formatting accepted by this tool:

\<BundleID\>\n

Basically the BundleID followed by a newline. You can take a look at the included `BaseBIDs.txt` so you can form your own list.

The included `BaseBIDs.txt` is about ~200 entries big and includes common app's BundleIDs (from my perspective).


## Credits / Thanks
- Apple for their iTunes Search API
- offcornerdev for the original site & inspiring me to work on this tool.
- Some random pigeon I saw in town recently, you rock homie!


Feel free to follow me on Twitter [@Saadat603](https://twitter.com/Saadat603) !

Have fun and enjoy!
