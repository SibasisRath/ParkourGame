public enum ResultType
{
    None,

    Winner,
    Not_A_Cat_Person, // Suvived without cats
    Cat_Person, // saved all cats
    Last_Minutes_Survivor, // completed last minutes 

    Did_Not_Win,
    Tried_And_Died, // If player died with avg parkour actions 

    Chill_Player, // 0 dash 
    Quick_Moves, // high dash counter
    Parkour_Addict // high number of parkour action done
}
