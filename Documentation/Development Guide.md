# Development Guide

## Getting The Android Application Package (.apk)

The application can be manually installed onto a Meta Quest headset using a .apk file.

- The .apk can be downloaded if the user is added to the private release channel.
- Provided by the developers.
- Built from the Unity project.

### Disclaimer

If the application is installed through using an .apk, **Spatial Data** cannot be shared across devices, unless the account associated with the Meta Quest headset is added to the **private release channel** discussed in the [User Guide](./User%20Guide.md). The application may also be the same verison as the one on the release channel, causing issues. Using a release channel will prevents these issues.

## Installing Using Meta Quest Developer Hub (MQDH)

### Prerequisites

- [**Meta Horizion Mobile App**](https://horizon.meta.com/)
- [**Meta Quest Developer Hub**](https://developers.meta.com/horizon/documentation/unity/ts-odh/)

### Steps

1. Follow the [steps to setup MQDH](https://developers.meta.com/horizon/documentation/unity/unity-quickstart-mqdh)
2. In **Device Manager** drag and drop the .apk file or click **Add Build** and select the .apk file
3. The application will be installed on the Meta Quest headset.

## Unity

### Prerequisites

- [**Unity**](https://unity.com/)
- [**Meta Horizion Mobile App**](https://horizon.meta.com/)
- [**Meta Quest Developer Hub**](https://developers.meta.com/horizon/documentation/unity/ts-odh/)
- [**Meta Quest Link**](https://www.meta.com/au/quest/setup/)
- [**Windows PC With Minimum Requirements**](https://www.meta.com/en-gb/help/quest/articles/headsets-and-accessories/oculus-link/requirements-quest-link/)
- **USB-C Cable**

### Steps

#### Meta Quest Link

1. Setup Meta Quest Link with the Meta Quest headset
2. Goto **Settings > General > Unknown Sources > Enable**
3. Goto **Settings > Beta > Developer Runtime Features > Enable All**

#### Setting Up & Building The Project

1. Install [**Unity Hub**](https://unity.com/download)
2. Clone the this repository

```bash
git clone https://github.com/Philllipe/mr-aircraft-tool.git
```

3. In Unity Hub, click **Add** and select the cloned repository
4. Install the specified version of Unity and ensure...
   - **Andriod Build Support** is installed
   - The platform is Android
   - Later versions of Unity may still work, but the project was developed using Unity 2022.3.39f1
5. Open the project in Unity, you may be asked to restart Unity
6. Goto **File > Build Settings** and ensure...
   - Platform is set to **Android**
   - Run Device is set to **Meta Quest** headset
   - The '**main**' scene is added to the build
7. Click **Build** or **Build And Run**
   - If you click **Build**, the .apk will be saved in the specified location. You can follow the [Installing Using Meta Quest Developer Hub (MQDH)](#installing-using-meta-quest-developer-hub-mqdh) steps to install the .apk on the Meta Quest headset
   - If you click **Build And Run**, the .apk will be installed on the Meta Quest headset and ran automatically
   - If you recieve **can not sign the application**
     - Goto **Edit > Project Settings > Player > Publishing Settings > Custom Keystore > Disable**

## Photon Fusion

- Photon Fusion 2 is the networking solution used in this project
- In order to enable networking within the application a valid **Fusion App ID** must be added to **Tools > Fusion > Fusion Hub** in Unity
  - The fusion App can be created in the [**Photon Dashboard**](https://dashboard.photonengine.com/)
  - The current **Fusion App ID** is provided will be deprecated, as such this will need to be updated.

## Shared Spatial Anchors

- See details in [Release Channel](./User%20Guide.md), [Sharing Spatial Data & Multiplayer](../README.md#sharing-spatial-data--multiplayer) as well as the Meta documentation to understand this.

## Documentation

- Addtional configuration documentation can be found in [Configuration](./Configuration.md)
- [**Meta Developer Documentation**](https://developers.meta.com/horizon/documentation/unity)
- [**Fusion 2**](https://doc.photonengine.com/fusion/current/fusion-intro)

## Models

- [**Unity Supported Model File Formats**](https://docs.unity3d.com/Manual/3D-formats.html)
- Models used in this project
  - [C130](https://sketchfab.com/3d-models/fab-2471-c-130-hercules-free-f5fae27c4270472fac399055699facb4)
  - [Blackhawk](https://sketchfab.com/3d-models/uh-60-blackhawk-16b7614eb6c741d096641bb89a043bfb)
  - [Airplane Engine](https://sketchfab.com/3d-models/airplane-engine-bb658020350e461aa8d915bc58cd6ef9)
  - [X-Wing](https://sketchfab.com/3d-models/x-wing-a185c8bb6e9d43e4b597b856b176d768)
