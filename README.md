![Blobber_Desk_4](https://github.com/Smaths/ScoreSpaceJam26/assets/13316137/626cb5ae-0104-4400-8c47-aa86c07dcaf0)

# Blobber
This game was born out of the [ScoreJam #26 game](https://itch.io/jam/scorejam26) game jam hosted on [itch.io](https://itch.io/) 3 day game jam that expanded into a 3 week project due to the fact that it was so darn cute. This was the first game jam for Jonas & Eric duo 🤜💥🤛. It was a great opportunity to implement several intereting systems that I can reuse in future projects. 

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

## Some Systems Implemented:
1. 🤿 **Object Pooling**: Improve performance when managing the creation/destruction of objects in a scene. This is must-know for game dev, it's pretty straightforward and has such a great impact on framerate.
2. 🤖 **State Machine**: I'm so excited to implement a state machine for the characters in this game. The state machine is improves on the massive character controller classes with nested `if` statements by allowing you to only concern yourself wtih the current state. It's wonderful and something I expect to use significantly going forward.
3. 📻 **Audio SFX Manager**: This feature was born out of the issue that WWise audio middleware doesn't support WebGL builds (something Jona and I didn't realized until we fully implemeted everything using WWise xD). So I built an audio SFX manager based on the [tutorial from Sirenix's youtube page](https://www.youtube.com/watch?v=bJ3Bu9kpZAA) (company responsible for the popular Odin plugin for Unity game engine).
4. 🏔 **Unity Terrain**: Finally spent some time learning the main features of the terrain. I'm really pleased with the flexibilty and speed of it. I can iterate quickly, which is really important. My hours making D&D maps for VTT games has come into handy here with blending textures 🦄.
5. 🧭 **NavMesh AI**: Another native system from Unity that I wanted to explore. This is a really great and simple system for moving the blobs around the map and handing avoidance of obstacles and the like. I feel like I can leverage some of the handy API calls even better to improve the logic of my blobs. 

## Credits
| Person | Role| 
| --- | --- | 
| 🧙‍♂️ Eric Wroble | artist*, programmer, game design |
| 👨‍🚀 Jonas Bengio | sound, game design | 

_*Some 3rd party assets used (for example the blobs)._

## Links
- [Play the Game](https://play.unity.com/mg/other/webgl-builds-356030) on Unity (WebGL)
- [ScoreSpace Jam #26 details](https://itch.io/jam/scorejam26)
