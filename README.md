![Blobber_Desk_4](https://github.com/Smaths/ScoreSpaceJam26/assets/13316137/626cb5ae-0104-4400-8c47-aa86c07dcaf0)

# Blobber
The Blobber game was born out of the [ScoreJam #26 game](https://itch.io/jam/scorejam26) game jam hosted on [itch.io](https://itch.io/) 3 day game jam that expanded into a 3 week project due to the fact that it was so darn cute. This was the first game jam for Jonas & Eric duo 🤜💥🤛. It was a great opportunity to implement several interesting systems that we can reuse in future projects. 

From a game design perspective: we wanted the game to be simple, fast and cute. The goals was to enjoy ourselves and create something with whimsy and delight. It's a short game (60 second rounds), but we're really pleased with the result.

This Unity Project is freely available, though it does use some paid 3rd party assets (Odin Inspector & Serializer) for which you will need you own license to use. That being said, feel free to fork it as well as make suggest changes or improvements. We can't promise anything, but we are happy to entertain all sorts of strange ideas. 

We hope you enjoy this little romp. 

## How to Play 
⚪️ Play as a blob. 

🟡 Consume yellow blobs to grow bigger and gain points. 

🔴 Avoid red blobs or they will take your points and make you shrink. 

### Controls
- **W,A,S,D** - Move
- **Spacebar** - Boost
- **Esc** - Pause

## Screenshots

<table style="padding:5px">
  <tr>
    <td><img src="https://github.com/Smaths/ScoreSpaceJam26/assets/13316137/5b06535e-c883-44a6-a223-0a3aca838b62" alt="Blobber Home Screen" width = 512px></td>
    <td><img src="https://github.com/Smaths/ScoreSpaceJam26/assets/13316137/dc0a3a2c-e451-4802-94fd-16a34df7e93c" alt="Blobber Gameplay Screenshot One" width = 512px></td>
  </tr>
  <tr>
    <td><img src="https://github.com/Smaths/ScoreSpaceJam26/assets/13316137/f26d5df6-4567-4b82-a8ae-fbf34e61f825" alt="Blobber Gameplay Screenshot Two" width = 512px></td>
    <td><img src="https://github.com/Smaths/ScoreSpaceJam26/assets/13316137/1cc8f6da-9572-42dd-af35-c91ccc8817e3" alt="Blobber Gameplay Screenshot Three" width = 512px></td>
  </tr>
</table>

## Systems Implemented
This was a great project to implement some common game development systems and patterns. I made an effort to keep the components modular and lightweight for easy resuse in future projects. 
1. 🤿 **Object Pooling**: Improve performance when managing the creation/destruction of objects in a scene. This is must-know for game dev, it's pretty straightforward and has such a great impact on framerate.
2. 🤖 **State Machine**: I'm very pleased to implement a state machine for the characters in this game. The state machine is improves on the massive character controller classes with nested `if` statements by allowing you to only concern yourself with the current state. It's wonderful and something I expect to use frequently.
3. 📻 **Audio SFX Manager**: This feature was born out of the issue that WWise audio middleware doesn't support WebGL builds (something Jona and I didn't realized until we fully implemeted everything using WWise xD). So I built an audio SFX manager based on the [tutorial from Sirenix's youtube page](https://www.youtube.com/watch?v=bJ3Bu9kpZAA) (company responsible for the popular Odin plugin for Unity game engine).
4. 🏔 **Unity Terrain**: Finally spent some time learning the main features of the terrain. I'm really pleased with the flexibilty and speed of it. I can iterate quickly, which is really important. My hours making D&D maps for VTT games has come into handy here with blending textures 🦄.
5. 🧭 **NavMesh AI**: Another native system from Unity that I wanted to explore. This is a really great and simple system for moving the blobs around the map and handing avoidance of obstacles and the like. I feel like I can leverage some of the handy API calls even better to improve the logic of my blobs.
6. 📈 **Leaderboards**: The initial game jam was sponsored by [LootLocker](https://lootlocker.com/) who provide a really convenient SDK to help implement and host leaderboard data into your Unity game. I found it really easy to work with and has a bunch more features I'd like to explore for storing player data. 

## Credits
| Person | Role| 
| --- | --- | 
| 🧙‍♂️ Eric Wroble | artist*, programmer, game design |
| 👨‍🚀 Jonas Bengio | sound, game design | 

_*Some 3rd party assets used (for example the blobs)._

## Links
- [Play the Game](https://play.unity.com/mg/other/webgl-builds-356030) on Unity (WebGL)
- [ScoreSpace Jam #26 details](https://itch.io/jam/scorejam26)
