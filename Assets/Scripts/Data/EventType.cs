// Enum with all the events in the game
public enum EventType
{
    // SceneManagement                       
    FADING, 
    QUIT_GAME,  

    // Inputs
    PLAYER_1_MOVE_VECT,
    PLAYER_2_MOVE_VECT,

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
    LVL1_DOOR_FINALPUZZLE,      // To remove door from final puzzle to get to end of level

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

    // Level
    LEVEL_SPAWN,
    SOFTPUZZLE_PLAYER_TELEPORT, // The one that creates the VFX effect
    SOFTPUZZLE_COMPLETE,

    // Gravity 
    GRAVITY_INVERT,

    // Testing
    TEST_CONTROLS,
};