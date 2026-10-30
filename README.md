# Enhanced Create with VR

My project for the Enhanced Create with VR assignment. It opens on a main menu and has two rooms you can move between without restarting the app:

- **Main Menu**, where you pick a room
- **Bruno Room**, the living room I built while following Unity Learn's Create with VR course
- **VR Room**, a small space station built from the VR Room assets on the Intranet, with a glass door you open by its handle

Unity 6000.0.80f1, URP, XR Interaction Toolkit 3.1.3, OpenXR, built for the Meta Quest 2.

Video (playing it in the headset, then the Unity setup and code): https://www.youtube.com/watch?v=HcwbF1lmywM

## Running it

1. Open the folder in Unity Hub with Unity 6000.0.80f1 (Android Build Support installed).
2. Open `Assets/Scenes/Main Menu.unity` and press Play with a headset connected through Quest Link, or build to the Quest from File > Build Profiles (Android).
3. All three scenes are in the build list: Main Menu is scene 0 and starts first, Bruno Room is scene 1, VR Room is scene 2.

## Controls

| Button | What it does |
| --- | --- |
| Grip | Grab, and curl the hand's fingers |
| Trigger | Use the held object (flashlight, remote, lighter...), and curl the index finger |
| A / X, or touch a thumbstick | Show the teleport ray. Aim at the floor or a mat and let go to teleport |
| Right thumbstick left/right | Snap turn (can be switched off in Settings) |
| Left thumbstick left/right | Smooth turn |
| Either thumbstick forward/back | Walk, when Continuous Movement is on in Settings |
| B | Open the menu |

The menu has Settings, Reset (Restart in the VR Room), a button to go to the other room and a Main Menu button. Settings has snap turning, distance grab, continuous movement and closed captions. You can point at buttons with the hand ray and pull the trigger, or poke them with a finger.

## Course lessons completed

I finished all of Units 1 to 3 and the three challenges.

- Unit 1: project setup and building to the Quest, locomotion (teleport area and anchors, snap and continuous turn), grabbable objects, sockets, Challenge 1 (Architecture)
- Unit 2: audio and haptics, activate interactions, near-far interactors, user interface, Challenge 2 (3D Painting)
- Unit 3: comfort settings, optimization, baked lighting, building and publishing, Challenge 3 (Training)

The fixed challenge scenes are in `Assets/Challenges`. The personal project labs are in a separate project. The course completion certificate is in `Certificate`.

What the course gave Bruno Room: a modern room with a city view, teleporting with snap and smooth turning, a tennis ball and racket, fruit, hats you can try on, paintings you can move between hooks, a TV remote that plays a video on the TV, a phone, flashlight, lighter and candle, dart gun, polaroid camera, watering can, a cabinet with hinged doors, spatial audio and haptics, a record player, closed captions, welcome and settings menus with fade in, and baked lighting with light probes and a reflection probe.

## What I added for this assignment

### Main Menu scene (`Assets/Scenes/Main Menu.unity`)

The app starts here. You stand on a round mat among the course's green hills, with quiet music (Beethoven's Moonlight Sonata), in front of a panel titled "Bruno's VR World" with two buttons: Bruno Room and VR Room. You point at a button with the hand ray and pull the trigger.

I made it from a copy of Bruno Room and kept only the XR rig and the interface, so the hands, rays, poke, fade and Dark UI look are the same as in the rooms. There's no teleport floor here, only the mat you stand on, and B does nothing because there is no other menu to open. When the scene loads, the course's `TeleportPlayer` puts you on the mat facing the menu. The scene has a realtime sun, so it needs no baked lighting.

### VR Room scene (`Assets/Scenes/VR Room.unity`)

I copied the VR Room assets into `Assets/VR Room Assets` and converted their materials from the built-in shaders to URP (Standard ones with URP's own material upgrader, the leaves, origami and hologram particle materials by hand). The scene came with a camera only, and a lot of missing scripts left over from the project it was exported from, which I removed.

The layout: you start in the storage room. The glass door at its north end leads through a corridor to the lounge.

| Interaction | Where it is |
| --- | --- |
| Start UI | Welcome panel in front of you when the scene opens, with the instructions and a Start button that closes it |
| Physics door | The glass door at the end of the storage room. Grab the bar handle on either side. It opens one way, into the lounge |
| Grabbing | Boxes, tubes and containers on the storage shelves, the rope, the flashlight on a crate in the corner behind you on the left (trigger turns it on), the coffee mug on its coaster at the back corner of the lounge desk (tip it and the coffee pours out), the origami crane on the projector |
| Sockets | The chess board on the glass table at the far end of the lounge. Four pieces are missing and are hidden around the station: the white knight on the storage shelf, the black rook on the storage room floor by the corridor, the black pawn on the lounge floor by the plants, the white bishop on the top bunk by the beds. The sign gives a hint, and when all four are back it says "Well done!" and a success sound plays. The mug's coaster is a fifth socket that only takes the mug |
| In-room UI | B menu (Settings, Restart, Bruno Room, Main Menu), the settings panel, and the sign over the chess board |
| Videos | The three monitors on the lounge desk play the course videos. Only the middle one has sound |
| Teleporting | The whole floor is a teleportation area. The Start Mat and the Chess Mat are teleportation anchors |
| Clocks | Analog and digital clock on the storage room wall, left of the glass door |
| Sound | A machine humming by the big battery in the storage room, Bach playing in the lounge, and captions for both |

### Bruno Room changes

- Realistic animated hands instead of the blue course hands
- A digital clock next to the analog clock on the wall above the TV
- B now opens a menu with Settings, Reset Room, VR Room and Main Menu (reset still asks Yes/Cancel first)

### Both scenes

The VR Room uses a copy of Bruno Room's XR rig and interface, so grabbing, teleporting, turning, the settings, haptics, click sounds and the fade are the same in both. The menus in both rooms use the same look, made with the free Dark UI pack: dark rounded panels with a thin blue frame and blue buttons.

## Scene navigation

The app starts in the Main Menu, where you pick a room. In either room, press B and pick the other room or Main Menu. Every one of these buttons calls `LoadScene.LoadSceneUsingName` from the course library with the scene's name ("Bruno Room", "VR Room" or "Main Menu"), which loads that scene with `SceneManager.LoadScene`.

Every scene has its own complete rig (XR Origin, Input Action Manager, XR Interaction Manager and EventSystem). Nothing is kept with DontDestroyOnLoad, so the old scene is unloaded completely and the new one starts with a fresh rig. There are never two XR Origins or two event systems, and input, grabbing and UI work the same after every switch. When a scene loads, `OnSceneLoad` on the Scene Loader object calls `FadeCanvas.StartFadeOut`, so the view fades in from black instead of jumping.

Every scene also puts you in the right place when it loads. The headset's position is measured from the centre of your play area, so without help you'd appear wherever you're standing in your real room, and in the small storage room that can be outside the walls. The course's `TeleportPlayer` on the XR Origin asks the Teleportation Provider to teleport you onto the scene's start mat in its `Start`, facing the menu. For that to work the Locomotion object has to be on when the scene loads, so it's on in all three scenes.

## Code walkthrough

### Animated hands (`Assets/Scripts/HandAnimator.cs`)

The hand models are LeftHand.fbx and RightHand.fbx from the HandVisualizer sample of Unity's XR Hands package, with a skin material. They're saved as `Assets/Prefabs/Left Hand Model.prefab` and `Right Hand Model.prefab`, and each one sits under its Hand Controller (the object with the Tracked Pose Driver) at the same position and roll as the course hand it replaced.

`HandAnimator` has two `InputActionProperty` fields. In the prefabs they point at `XRI Left/Right Interaction/Activate Value` (trigger) and `Select Value` (grip) in the XRI Default Input Actions. The Input Action Manager in the rig enables that action asset, so the values are live.

In `Awake` it finds the finger joints by name (`IndexProximal`, `MiddleDistal` and so on) and saves their starting rotations. In `Update` it reads both values, moves towards them a little each frame so the fingers don't snap, and sets every joint to `start * Quaternion.Euler(angle, 0, 0)`. The joints bend around their own X axis, which curls them towards the palm. The trigger bends the index finger, the grip bends the other three fingers and the thumb.

The course hands were hidden while you held something. I removed those events, so the new hands stay visible and close around what you hold.

### Analog and digital clock (`AnalogClock.cs`, `DigitalClock.cs`)

`AnalogClock` reads `DateTime.Now` every frame. Minutes include the seconds and hours include the minutes, so the hands move smoothly:

```csharp
float minutes = now.Minute + now.Second / 60f;
float hours = now.Hour % 12 + minutes / 60f;
SetHand(hourHand, hourStart, hours * 30f);     // 360 / 12 hours
SetHand(minuteHand, minuteStart, minutes * 6f); // 360 / 60 minutes
SetHand(secondHand, secondStart, now.Second * 6f);
```

Each hand is set to its starting rotation times a turn around its Y axis, which points out of the clock face. No animation clips are used. `DigitalClock` writes `now.ToString("HH:mm:ss")` and the date into two TextMeshPro texts, only when the second changes. The digital clock is a prefab (`Assets/Prefabs/Clock_Digital.prefab`) used in both rooms.

### The glass door

The door model was a sliding door played by an Animator. I removed the Animator and turned it into a hinged door:

- The door leaf (`glass_panel_1_door`) has a Box Collider, a Rigidbody (mass 3, no gravity, angular damping 5) and a Hinge Joint. The joint's anchor is on the leaf's own pivot, which is on its hinge edge, the axis is up, limits are -90 to 0 so it only opens one way, into the lounge, like a real door (0 is closed, so it stops against the frame), and it has no connected body, so it's attached to the world and can't come off.
- The frame's mesh collider is replaced by three Box Colliders (both sides and above the doorway) that leave a few centimetres free around the leaf, so the door doesn't rub on its own frame.
- A Handle child sits on the bar of the door model. It has a Box Collider around the bar on both faces, its own Rigidbody, a Fixed Joint to the leaf and an XR Grab Interactable: movement type Velocity Tracking, Track Rotation off, Throw On Detach off, Far Attach Mode Far, Use Dynamic Attach on, velocity scale 0.6. These are the same settings as Bruno Room's cabinet door handles.

While you hold the handle, velocity tracking sets the handle's velocity towards your hand on every physics step. The fixed joint pulls the leaf with it and the hinge only lets the leaf turn around the hinge line, so the door follows your hand continuously. When you let go, the angular damping stops it. Far attach and dynamic attach keep the handle where you grabbed it instead of pulling it to your hand, which would yank the door. I tested this in the editor by moving the handle around the hinge the same way velocity tracking does, and the door followed within 1 degree all the way to 75 degrees. I also pushed the handle from each side in a physics test: pushed from the storage room it opens to 90 degrees, pushed from the lounge side it stays shut.

### Sockets and filtering

Each of the four sockets on the chess board is an XR Socket Interactor with a small trigger Sphere Collider, placed exactly where I removed that piece from the board, with the board piece's rotation. The pieces snap into that position and orientation.

What a socket accepts is decided by interaction layers. I added the layers Knight, Rook, Pawn and Bishop. The knight's XR Grab Interactable has Default + Knight, and the knight socket's Interaction Layer Mask is Knight only. An interactor and an interactable can only interact if their masks share a layer, so the knight socket can't take the rook, the pawn or a box. The hands use Default, so they can still grab every piece and take it back out of a socket. Each socket plays a click (`PlayQuickSound`) on Select Entered.

`SocketGoal` (`Assets/Scripts/SocketGoal.cs`) sits on the Chess Sockets object and has the four sockets in a list. Every frame it checks `hasSelection` on each one, and the first time all four are holding a piece it runs its `OnAllFilled` event. In the scene that event hides the task text on the sign, shows "Well done!" and plays `SFX_Game_Success`.

The mug's coaster works the same way. I added a fifth interaction layer, Mug. The mug has Default + Mug and the Mug Socket on the coaster has Mug only, so no chess piece or box can go on the coaster, and the mug can't go in a chess socket. The socket's Starting Selected Interactable is the mug, so the mug starts on its coaster. I moved the mug and coaster to the back corner of the desk, about a metre from the keyboards, so it can't spill on them.

### Grabbing

Every grabbable has a Rigidbody, a non-trigger collider and an XR Grab Interactable with dynamic attach. Small things (chess pieces, mug, crane, flashlight) use Instantaneous movement. Boxes, tubes and containers use Velocity Tracking, so they bump into walls and shelves while you hold them. The hands grab with a Near-Far Interactor (close up, or with the hand ray) and play a haptic pulse and a click when you grab.

### Sound after Build and Run (`RestartAudioOnFocus.cs`)

The Quest mutes an app while the headset is off your head. With Build and Run the app starts while the headset is still on the desk, and I found (by reading the Quest's logs over USB) that its sound stream then stayed muted even after I put the headset on, so there was no sound at all. `RestartAudioOnFocus` sits on the XR Origin in every scene. In `OnApplicationFocus` (line 13), every time the app gets focus back it waits a second (line 23) so the Quest has unmuted the app, then calls `AudioSettings.Mobile.StopAudioOutput` and `StartAudioOutput` (lines 25 to 27). That opens a fresh sound stream, which isn't muted. I tested it on the headset: started the app with the headset on the desk, put it on, and the music played.

### Coffee mug

The mug works the same way as the watering can in Bruno Room, with course scripts only:

- A thin dark brown disc (`Coffee`) sits just under the rim, so the mug looks full.
- `Particle_Coffee` is the course's `Particle_Water` with a brown copy of its material (`Assets/Art/Coffee`). It sits at the rim and points out of the mug.
- `OnTilt` on the mug checks how far the mug's up direction has turned towards the floor. Past the threshold (0.3, about 66 degrees) its On Begin event plays the particle (`ToggleParticle.Play`), starts the pouring sound (`PlayContinuousSound`, the same clip as the watering can) and hides the coffee disc. On End stops both and shows the disc again when you stand the mug back up.

### Monitor videos

Each monitor on the lounge desk has a quad placed exactly on its glass, with a Video Player and the course's `PlayVideo`. The Video Player is set to Material Override with `_BaseMap`, so the video frames go straight onto the quad's material. `PlayVideo` swaps in an unlit material when it starts so the video isn't darkened by the room's lighting. Play At Start and Loop are on. The middle monitor sends its sound to a 3D Audio Source that fades out within 5 metres. The side monitors are muted, so the lounge doesn't get noisy.

### Teleporting

Teleporting needs a Teleportation Provider in the rig, under Locomotion with the Locomotion Mediator and XR Body Transformer. It also needs a ray interactor whose interaction layers overlap the target's (both use my Raycasts layer), and a target with a collider the ray can hit. You hold A/X or touch a stick to show the ray, aim, and let go to select.

A Teleportation Area lets you land anywhere on its colliders: in the VR Room it's on the Floors object, using all 25 floor colliders. A Teleportation Anchor always puts you on one fixed spot, facing its forward direction: the Start Mat and the Chess Mat. B uses the course's `TeleportPlayer` to send you back to the Start Mat in front of the menu, the same as in Bruno Room. The closed glass door blocks the teleport ray, so you have to open it to get to the lounge.

### UI

The canvases are world space with a Tracked Device Graphic Raycaster, and the EventSystem uses the XR UI Input Module, so the hand rays and the poke interactors can press buttons. B is an `OnButtonPress` (course script) on the XR Origin, listening to the right controller's secondary button. Its events turn the menu on, hide the other panels and teleport you to the mat. The Start button hides the welcome panel and plays a sound.

The panels and buttons use sprites from the Dark UI pack. The sprites are white, so the Image color decides the final color. I gave the filled panel and button sprites 9-slice borders in their import settings, and set Pixels Per Unit Multiplier on each Image, so the round corners keep their shape at any size. The buttons use Color Tint transitions: blue normally, grey for Cancel and red for the reset Yes, a bit lighter when you point at them and darker when you press. The four buttons in the B menu share one text size, small enough that "Bruno Room" fits on one line.

## Project layout

| Folder | What's in it |
| --- | --- |
| `Assets/Scenes` | Main Menu, Bruno Room and VR Room, with the rooms' lighting settings and baked lighting |
| `Assets/Scripts` | My scripts: HandAnimator, AnalogClock, DigitalClock, SocketGoal, RestartAudioOnFocus, ReticleAnimator, BounceSound, RecordSpinner, plus the lesson's Polaroid and Photo |
| `Assets/Prefabs` | Hand models and the digital clock |
| `Assets/Art` | Hand models and skin material, coffee materials, paintings and materials |
| `Assets/VR Room Assets` | The VR Room assets from the Intranet, with URP materials |
| `Assets/Dark UI` | The free Dark UI pack, used for the menu panels and buttons |
| `Assets/_Course Library` | The course's assets and scripts |
| `Assets/AllSkyFree` | The free AllSky pack. Only the blue sky that Bruno Room and the Main Menu use is in the repo, the other skies are left out by `.gitignore` |
| `Assets/Challenges` | The fixed scenes from the three course challenges |
| `Assets/Samples`, `Settings`, `Resources`, `TextMesh Pro`, `XR`, `XRI` | Made by Unity and its packages (XR Interaction Toolkit samples, URP settings, Meta audio settings, TextMesh Pro, XR plug-in and interaction layer settings). They stay where the packages expect them |

In the hierarchy, Bruno Room is grouped under XR, LIGHTING, STATIC, DYNAMIC and INTERFACE. The VR Room keeps the asset's own Level and Interactables groups and adds XR, LIGHTING and INTERFACE.

## Other additions

- A Main Menu scene where the app starts, with a Main Menu button in both rooms' menus
- Fade in when a scene loads
- A welcome panel with the instructions and a Start button in the VR Room
- Sign over the chess board telling you what to do, and a click when a piece goes back in
- Flashlight that turns on and off with the trigger
- Settings menu (snap turning, distance grab, continuous movement, closed captions) in both rooms
- Sound in the VR Room: a machine hum in the storage room and Bach in the lounge (3D sound that fades with distance), a sci-fi sound on Start, and closed captions that turn to face you
- A goal: find the four missing chess pieces, with a hint on the sign, and a "Well done!" and a success sound at the end
- A coffee mug that pours when you tip it and sits on a coaster socket made only for it, and videos playing on the lounge monitors
- Menus restyled with the Dark UI pack in both rooms
- Baked lighting in the VR Room too, with light probes and two reflection probes
- The storage crates' colliders were taller than the crates, so things put on them floated. I fitted them to the crates.
- Two of the chess pieces from the asset were out of reach (one on top of the storage unit, one under a cushion you can't move). I moved them where you can find them.

## Design notes

- The door opens one way like a real door, into the lounge. It has no latch on purpose: it stays where you leave it, and from the lounge you pull it open by the handle.
- All sounds use Unity's own 3D sound (Spatial Blend set to 3D), so they get quieter with distance and come from the left or right.
- The hands follow the controllers instead of using hand tracking, because the controller buttons are needed for teleporting, turning and the menu.
- The B button on the right controller opens the menu and also brings you back to the start mat, so the menu always opens in front of you.
