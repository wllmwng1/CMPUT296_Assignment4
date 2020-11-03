using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChurnPredictor
{
    //TODO; FIX THIS FUNCTION FOR EXTRA CREDIT, TEST WITH "Check Accuracy" BUTTON
    public Datapoint[] AssignPredictedChurn(Datapoint[] verificationDatapoints, Dictionary<Datapoint, List<Datapoint>> clustersByCenters)
    {
        foreach(Datapoint d in verificationDatapoints)
        {
            if (d.Level == 1)
            {
                d.SetPredictedChurn(false);
            }
            else
            {
                if (d.AvgScore > 1000)
                {
                    d.SetPredictedChurn(false);
                }
                else
                {
                    d.SetPredictedChurn(true);
                }
            }
            
        }
        
        return verificationDatapoints;         
    }
}
