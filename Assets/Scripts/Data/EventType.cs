// Enum with all the events in the game
public enum EventType
{
    // SceneManagement                       
    FADING, 
    QUIT_GAME,  

    // Inputs
    PLAYER_1_MOVE_VECT2D,
    PLAYER_2_MOVE_VECT2D,

    PLAYER_1_INTERACT,
    PLAYER_2_INTERACT,

    // Puzzles
    ASSIGNMENT_CODE_TRIGGER,  //trigger code for puzzle features
};