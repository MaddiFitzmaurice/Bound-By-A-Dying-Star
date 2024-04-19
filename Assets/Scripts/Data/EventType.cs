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
    PLAYER_2_INTERACT,

    PLAYER_1_RIFT,
    PLAYER_2_RIFT,

    PLAYER_1_SENDITEM,
    PLAYER_2_SENDITEM,

    PLAYER_1_NPC,
    PLAYER_2_NPC,

    // Puzzles
    ASSIGNMENT_CODE_TRIGGER,    //trigger code for puzzle features
    PUZZLE_DONE,                // trigger if a puzzle is done to be sent to the puzzle controller

    // Narrative
    NPC_SEND_DIALOGUE,

    // RiftManager
    CREATE_RIFT,
    RIFT_SEND_EFFECT,

    // Camera
    LEVEL_CAMS_REQUEST_FOLLOWGROUP,
    LEVEL_CAMS_SEND_FOLLOWGROUP,
    LEVEL_CAMS_YROT,

    // Teleport
    TELEPORT_PLAYERS,

    // Gravity 
    GRAVITY_INVERT,

    // Testing
    TEST_CONTROLS,
};