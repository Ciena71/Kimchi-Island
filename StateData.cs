using System;

[Serializable]
public class StateData
{
    public int popularity;
    public int predictedPopularity;
    public SupplyDemand[] supplyDemand = new SupplyDemand[86];
    public int updated;
    public bool edited;
}

[Serializable]
public class SupplyDemand
{
    public int id;
    public int supply;
    public int demand;
}