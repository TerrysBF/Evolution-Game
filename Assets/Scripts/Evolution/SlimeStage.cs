using System;

[Serializable]
public class SlimeStage
{
    public string stageName;
    public UnityEngine.GameObject model;   // modelo/mesh de esa forma del slime
    public ResourceCost[] evolutionCost;   // costo para pasar de ESTA etapa a la siguiente
}
