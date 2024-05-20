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

    // Puzzles
    ASSIGNMENT_CODE_TRIGGER,    //trigger code for puzzle features

    // Level 1
    LVL1_STAR_ACTIVATE,         // used to get the pedestals to make their associated stars also shoot their beams

    // Narrative
    NPC_SEND_DIALOGUE,

    // RiftManager
    CREATE_RIFT,
    RIFT_SEND_EFFECT,

    // Camera
    CLEARSHOT_CAMS_SEND_FOLLOWGROUP,
    CLEARSHOT_CAMS_YROT,
    RECEIVE_GAMEPLAY_CAM_PARENT,
    ADD_GAMEPLAY_CAM,
    DELETE_GAMEPLAY_CAM,

    // Teleport
    TELEPORT_PLAYERS,

    // Cinematics
    PLAY_CINEMATIC,

    // Level
    LEVEL_SPAWN,
    SOFTPUZZLE_PLAYER_TELEPORT, // The one that creates the VFX effect
    SOFTPUZZLE_COMPLETE,

    // Gravity 
    GRAVITY_INVERT,

    // Testing
    TEST_CONTROLS,
};