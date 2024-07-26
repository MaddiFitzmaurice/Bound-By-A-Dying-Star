// Enum with all the events in the game
public enum EventType
{
    // Scene Management                       
    PLAY_GAME,
    FADING, 
    QUIT_GAME,  

    // Inputs
    ENABLE_INPUTS,
    DISABLE_INPUTS,

    PLAYER_1_MOVE,
    PLAYER_2_MOVE,

    PLAYER_1_INTERACT,
    PLAYER_1_HOLDINTERACT,
    PLAYER_2_INTERACT,
    PLAYER_2_HOLDINTERACT,

    PLAYER_1_RIFT,
    PLAYER_2_RIFT,

    // Level 1
    LVL1_STARTWINKLE_ACTIVATE,          // used to get the pedestals to make their associated stars start twinkling
    LVL1_STARBEAM_ACTIVATE,             // used to get the pedestals to make their associated stars also shoot their beams

    // Narrative
    NPC_SEND_DIALOGUE,

    // Players
    PLAYERMANAGER_SEND_PLAYER1,         // Send the player1 object
    PLAYERMANAGER_SEND_PLAYER2,         // Send the player2 object
    PLAYERMANAGER_SEND_FOLLOWGROUP,     // Send the transform that Cinemachine will use to track both players
    PLAYERMANAGER_REQUEST_FOLLOWGROUP,  // Request that PlayerManager send the followgroup transform

    // Camera
    CAMERA_NEW_FWD_DIR,                 // Send new forward direction vector to change what orientation forward is for input movement
    CAMERA_REGISTER,                    // Register Cam to CameraManager
    CAMERA_DEREGISTER,                  // Deregister Cam to CameraManager
    CAMERA_ACTIVATE,                    // Activate (make live) a registered Camera

    // Cutscene
    CUTSCENE_PLAY,
    CUTSCENE_FINISHED,

    // Level
    LEVEL_SPAWN,                        // Spawn players into level for the first time
    SOFTPUZZLE_PLAYER_TELEPORT,         // Teleport players after completing a soft puzzle
    SOFTPUZZLE_COMPLETE,

    // Gravity 
    GRAVITY_INVERT,                     // Send whether gravity is inverted for players

    // Testing
    TEST_CONTROLS,

    // Audio
    MUSIC,
    SFX,
    INTRO,
    ITEM_PICKUP,
    ITEM_DROP,
    BACKGROUND_MUSIC,
};