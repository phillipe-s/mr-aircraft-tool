# **Project Configuration**

**In order for this project to be functional in editor and on device there is some initial setup that needs to be done.**

---

# Getting Started

## Install Unity Hub & Editor

1. Download from [Unity Download](https://www.oculus.com/lynx/?u=https%3A%2F%2Funity3d.com%2Fget-unity%2Fdownload&e=AT0IdeuFNvdqE328CWd1POSLIpBVYBC29Q_ou92YSXXOo1nGNuwU1R1RK0OV5b0lst81fA17CZNbFhMkT7Zb8itoAS7i52tOxK_P3ESGD2K2h5xRoE-6sTJWnF8nh9mn_auNv5uKmuFZmFE4t3FG6Q) page
2. Make a Unity account
3. In Unity Hub open the **Installs** tab and click **Install Editor**
4. On **Add Modules**, under **Platforms**, select **Android Build Support** and then make sure **Android SDK & NDK Tools** and **OpenJDK** is checked
5. Complete install

## Headset Setup

1. Download the **Meta Quest** mobile app
2. Make an account an pair the headset
3. Wear the headset and finish the setup
4. Go to **Menu > Devices > Select The Headset > Headset Settings**
    1. Turn on developer mode
5. Connect the headset to computer (needs to be a compatible USB port)
    1. When prompted in the headset click **Always allow from this computer** to allow USB debugging

## Setup Meta Quest Link

1. Download and install the [Oculus App](https://www.oculus.com/download_app/?id=1582076955407037)
2. Go to **Settings > General**
    1. Make sure is checked…
        - Unknown Sources
        - OpenXR Runtime (if active, will be greyed out).
3. Go to **Settings > Beta**
    1. Check all **Developer Runtime Features**

## Setup Meta Quest Developer Hub

1. Download [Meta Quest Developer Hub](https://developer.oculus.com/downloads/unity/)
2. Login with the same account used on the Headset
3. Make sure the headset is connected 
4. Go to **Device Manager** and the headset should be connected
    - Setup a new device if not
5. Make sure the device and the computer are on the same network



# Working With Unity

## Setting Up A Unity Project

1. In Unity Hub add a **New Project.**
2. Select the **3D Core** template.
    - **(OPTIONAL)** Check **Connect to Unity Cloud.**
3. Create the Project.
4. Go to **File > Build Settings.**
    - Select **Android** from the **Platform** list and click **Switch Platform.**
5. **(OPTIONAL)** In the **Run Device** list, select the Meta headset.
    - If the headset is not there…
        - click **Refresh** from the **Run Device** list.
        - re-plug headset, make sure it’s connect to computer and **Refresh** again.
        - make sure the headset is updated.
        - restart the headset.
6. Go to **Edit > Project Settings > XR Plug-in Management**
    - Install XR Plugin Management
    - Check Oculus in **Windows, Mac & Linux** tab and **Android** tab
7. Import the [Meta XR All-in-One SDK](https://assetstore.unity.com/packages/tools/integration/meta-xr-all-in-one-sdk-269657) from the [Unity Asset Store](https://www.oculus.com/lynx/?u=https%3A%2F%2Fassetstore.unity.com%2Fpublishers%2F25353&e=AT1U6OIF5XKzK912PvLyOECTZWYcaqXNpm7Q7w0rp4dgTpz9Vr1mAQkh3GlhTWEG0jU5ZNiRmjszTc0IY_8BOSEMSUyENsKrJ7JCKEK2OzM7OYAp5C-BaUd3otmqBr_jqAnql9wlKLWqury-Rk9POg)
    
    > Note when actually doing a project should only use the SDKs that are needed
    > 
    1. Click **Add to My Assets**
    2. Click **Open in Unity**
    3. Install and agree to everything (may need to restart editor)
    - This only needs to be done once. If already done…
        1. Go to **View > Package Manager**
        2. Select The Packages dropdown (top left) and click **My Assets**
        3. Search for the **Meta XR All-in-One SDK** or whatever else and install the package
        4. It will now appear when selecting **In Project** from the dropdown menu
8. Should now see the option to set **Play Mode** to use [**Meta XR Simulator**](https://developer.oculus.com/documentation/unity/xrsim-intro/)
    
    > This is how you can test without the headset
    > 
    > 
    > ![1](.\Media\1.png "Example photo")
    > 
9. Go to **Edit > Project Settings > Meta XR**
    1. Apply all settings in the **Android** tab
    
    > There should also be a Meta icon in the bottom right for easier access
    > 
    > 
    > ![2](.\Media\2.png "Example photo")
    > 

## High Level Overview of Developing a Unity App

![3](.\Media\3.png "Example photo")

Developing a Unity App

## Running & Testing A Unity App

1. Directly run the scene in the Unity Editor by hitting the Play button
    
    > Only works with headset and a [Supported GPU](https://www.meta.com/en-gb/help/quest/articles/headsets-and-accessories/oculus-link/requirements-quest-link/)
    > 
    1. Activate Quest Link in the headset by connecting to PC
        - Must have the Meta Quest Link app running on PC
        - Does not require a cable, can use the Air Link
    2. Click run button in Unity
    3. The App will be running in the headset
2. Check the Meta Icon to run in the [Meta XR Simulator](https://developer.oculus.com/documentation/unity/xrsim-intro/)
3. Run the project as a standalone app in the headset
    
    > Only works with the headset (must be connected with cable to install app)
    > 
    1. Go to **File > Build Settings**
    2. Make sure **Run Device** is set to the Quest 3
    3. Click **Build and Run**
        - Optionally just **Build**
    4. The app will now be installed and running on the headset
        - Will also make a '.apk' file

## Building Blocks

- [Documentation](https://developer.oculus.com/documentation/unity/bb-overview/)

# Version Control (Git)

## Configure Unity

- Go to **Edit > Project Settings > Editor**
    - Set **Asset Serialization** to **Force Text**
- Go to **Edit > Project Settings > Version Control**
    - Set **Mode** to **Visible Meta Files**

## Max Sizes

### Files

- GitHub has [File size limits](https://docs.github.com/en/repositories/working-with-files/managing-large-files/about-large-files-on-github)
    - 100 MB is the maximum file size
- To go over this [Git LFS](https://git-lfs.com/) must be used

### Git LFS

- [**Download**](https://git-lfs.com/)

- **Adding Files Larger Than Git LFS Allows**
    - These files should be uploaded to an external cloud storage
    - They will have to be added to the project manually after cloning it
- **Using Git LFS**
    - Need to [configure Git LFS](https://docs.github.com/en/repositories/working-with-files/managing-large-files/configuring-git-large-file-storage) in a repository
    - Git LFS needs to create a pointer to a file in order to track it
        
        ```bash
        # choose a file type to track (create pointers to these files)
        git lfs track "*.<file extension>"
        
        # make the sure .gitattributes is tracked
        # this file is what tracks the file extensions used by Git LFS
        git add .gitattributes
        
        # use git as you normally would by adding, commiting and pushing
        git add .
        git commit -m "adding files"
        git push <remote> <branch>
        ```
        

## Unity Repository Structure

- There are 3 main folders when using version control with Unity
    - `Assets`
    - `Packages`
    - `ProjectSettings`
- These folders only contain information on what the project is using and will be installed into the `Libary` folder upon *project initialisation*
- Everything else will be ignore in the `.gitignore` file
- **Custom Files/Folders**
    - The **`Assets`** folder is what stores all the Project’s assets from [Unity Asset Store](https://assetstore.unity.com/)
        - This means it is easiest to have assets that are gotten from the [Unity Asset Store](https://assetstore.unity.com/)
  -  You can add custom folders e.g. `Models` and push this to the repository **bearing in mind the things stated in things stated in [Max Sizes](https://www.notion.so/Meta-Quest-Unity-b606d6445f4246d7a6ab9f84bf083c05?pvs=21)**