namespace SleepFactorsApp.Domain;

// Momento della giornata: influenza il peso nell'analisi (Sera = più vicino al sonno = peso maggiore)
public enum TimeSlot
{
    Mattino = 0,
    Pomeriggio = 1,
    Sera = 2
}
