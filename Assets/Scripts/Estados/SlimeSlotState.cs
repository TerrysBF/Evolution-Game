public enum SlimeSlotState
{
    Empty,      // No hay slime/huevo (no se ha creado el “proceso”)
    New,        // Recién creado desde la incubadora (lo usaremos después)
    Ready,      // Huevo listo para evolucionar
    Running,    // En proceso de evolución (cuando intentas evolucionar)
    Waiting,    // En pausa por falta de recursos (player debe farmear)
    Finished    // Slime llegó a su evolución final
}
