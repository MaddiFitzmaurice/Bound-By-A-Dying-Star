// Enum with all the events in the game
public enum EventType
{
    // Scene Management                       
    PLAY_GAME,
    FADING, 
    QUIT_GAME,  
    MAIN_MENU,

    // Inputs
    ENABLE_GAMEPLAY_INPUTS,
    ENABLE_MAINMENU_INPUTS,
    DISABLE_GAMEPLAY_INPUTS,
    DISABLE_MAINMENU_INPUTS,

    PLAYER_1_MOVE,
    PLAYER_2_MOVE,
    CAN_MOVE,

    PLAYER_1_INTERACT,
    PLAYER_1_HOLDINTERACT,
    PLAYER_2_INTERACT,
    PLAYER_2_HOLDINTERACT,

    SHOW_PROMPT_INTERACT,
    HIDE_PROMPT_INTERACT,
    SHOW_PROMPT_HOLD_INTERACT,
    HIDE_PROMPT_HOLD_INTERACT,

    // Level 1
    LVL1_STAR_ACTIVATE,                 // used to get the pedestals to make their associated stars appear
    LVL1_STARBEAM_ACTIVATE,             // used to get the pedestals to make their associated stars also shoot their beams

    // Narrative
    NPC_SEND_DIALOGUE,

    // Players
    PLAYERMANAGER_SEND_PLAYER1,         // Send the player1 object
    PLAYERMANAGER_SEND_PLAYER2,         // Send the player2 object
    PLAYERMANAGER_SEND_FOLLOWGROUP,     // Send the transform that Cinemachine will use to track both players
    PLAYERMANAGER_REQUEST_FOLLOWGROUP,  // Request that PlayerManager send the followgroup transform
    PLAYER1_ISOFFSCREEN,                // Send bool for whether player 1 is offscreen
    PLAYER2_ISOFFSCREEN,                // Send bool for whether player 2 is offscreen

    // Camera
    CAMERA_NEW_FWD_DIR,                 // Send new forward direction vector to change what orientation forward is for input movement
    CAMERA_REGISTER,                    // Register Cam to CameraManager
    CAMERA_DEREGISTER,                  // Deregister Cam to CameraManager
    CAMERA_ACTIVATE,                    // Activate (make live) a registered Camera

    // Cutscene
    CUTSCENE_PLAY,
    CUTSCENE_FINISHED,

    // UI
    ARTWORK_SHOW,                       // Show artwork in fullscreen
    ARTWORK_HIDE,                       // Hide fullscreen artwork

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
    MIRROR_PLACEMENT,
    PRESSURE_PLATE_PLAYER1_ON,
    PRESSURE_PLATE_PLAYER1_OFF,
    PRESSURE_PLATE_PLAYER2_ON,
    PRESSURE_PLATE_PLAYER2_OFF,
    PEDESTAL_ROTATION,
    BEAM_CONNECTION,
    CONSTELLATION_COMPLETE,
    BACKGROUND_MUSIC,
};