using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KClusterer
{
    const int K = 7;//TODO; set K to the optimal value that you've found via experimentation
    const int MAX_ATTEMPTS = 100;//Maximum number of clustering attempts, you may want to use this
    const float threshold = 1f;//Threshold for cluster similarity, you may want to use this and alter it if so

    //TODO; fix this function
    public Dictionary<Datapoint, List<Datapoint>> Cluster(Datapoint[] datapoints)
    {
        //This datastructure will hold the clusters as lists by their centers. 
        Dictionary<Datapoint, List<Datapoint>> clustersByCenters = new Dictionary < Datapoint, List < Datapoint >> ();

        //Select K random centers to start
        List<Datapoint> centers = new List<Datapoint>();

        //Choose K random points to be centers
        while (centers.Count<K)
        {
            // Generate a random index less than the size of the array.  
            int randomIndex = Random.Range(0, datapoints.Length);
            Datapoint randomCenter = datapoints[randomIndex];

            if (!centers.Contains(randomCenter))
            {
                centers.Add(randomCenter);
            }
        }

        List<Datapoint> oldCenters = new List<Datapoint>();
        int i = 0;
        var set = new HashSet<Datapoint>(centers);

        while (!set.SetEquals(oldCenters) && i < MAX_ATTEMPTS)
        {
            i = i + 1;
            oldCenters = new List<Datapoint>(centers);
            clustersByCenters = new Dictionary<Datapoint, List<Datapoint>>();

            //Instantiate clusters by these centers
            foreach (Datapoint center in centers)
            {
                clustersByCenters.Add(center, new List<Datapoint>());
            }

            //Map each datapoint to its closest center
            foreach (Datapoint pnt in datapoints)
            {
                Datapoint closestCenter = null;
                float minDistance = float.PositiveInfinity;

                foreach (Datapoint center in centers)
                {
                    float thisDistance = Distance(pnt, center);
                    if (thisDistance < minDistance)
                    {
                        closestCenter = center;
                        minDistance = thisDistance;
                    }
                }

                clustersByCenters[closestCenter].Add(pnt);
            }

            centers = new List<Datapoint>();

            foreach (Datapoint center in oldCenters)
            {
                Datapoint newCenter = GetAverage(clustersByCenters[center].ToArray());
                centers.Add(newCenter);
            }
            set = new HashSet<Datapoint>(centers);
        }

        

        return clustersByCenters;
    }

    //Calculate the difference between sets of centers
    private float DifferenceBetweenCenters(Datapoint[] centers1, Datapoint[] centers2)
    {
        List<Datapoint> mappedPoints = new List<Datapoint>();
        float totalDistance = 0;
        foreach(Datapoint c1 in centers1)
        {
            Datapoint minPoint = null;
            float minDistance = float.PositiveInfinity;

            foreach(Datapoint c2 in centers2)
            {
                if (!mappedPoints.Contains(c2))
                {
                    float thisDistance = Distance(c1, c2);

                    if (thisDistance < minDistance)
                    {
                        minDistance = thisDistance;
                        minPoint = c2;
                    }
                }
            }
            mappedPoints.Add(minPoint);
            totalDistance += minDistance;
        }
        
        return totalDistance;
    }

    //Calculate and returns the geometric median of the given datapoints
    public Datapoint GetMedian(Datapoint[] datapoints)
    {
        Datapoint medianPnt = null;
        float totalDistance = float.PositiveInfinity;

        for(int i = 0; i<datapoints.Length; i++)
        {
            float thisDistance = 0;
            for(int j=0; j<datapoints.Length; j++)
            {
                if (i != j)
                {
                    thisDistance += Distance(datapoints[i], datapoints[j]);
                }
            }

            if (thisDistance < totalDistance)
            {
                totalDistance = thisDistance;
                medianPnt = datapoints[i];
            }
        }

        return medianPnt;
    }

    //Calculate and returns the average of the given datapoints
    public Datapoint GetAverage(Datapoint[] datapoints)
    {
        Datapoint sumDatapoint = new Datapoint("", 0, 0, 0, 0, 0, 0, 0, 0);
        int churnedVal = 0;

        foreach(Datapoint d in datapoints)
        {
            sumDatapoint = new Datapoint("", sumDatapoint.HoursPlayed + d.HoursPlayed, sumDatapoint.Level + d.Level, sumDatapoint.PelletsEaten + d.PelletsEaten, sumDatapoint.FruitEaten + d.FruitEaten, sumDatapoint.GhostsEaten + d.GhostsEaten, sumDatapoint.AvgScore + d.AvgScore, sumDatapoint.MaxScore + d.MaxScore, sumDatapoint.TotalScore + d.TotalScore);

            if (d.Churned)
            {
                churnedVal += 1;
            }
            else
            {
                churnedVal -= 1;
            }
        }
        //Calculate averages
        int hoursPlayed = (int)(((float)sumDatapoint.HoursPlayed) / ((float)datapoints.Length));
        int level = (int)(((float)sumDatapoint.Level) / ((float)datapoints.Length));
        int pelletsEaten = (int)(((float)sumDatapoint.PelletsEaten) / ((float)datapoints.Length));
        int fruitEaten = (int)(((float)sumDatapoint.FruitEaten) / ((float)datapoints.Length));
        int ghostsEaten = (int)(((float)sumDatapoint.GhostsEaten) / ((float)datapoints.Length));
        float avgScore = (((float)sumDatapoint.AvgScore) / ((float)datapoints.Length));
        int maxScore = (int)(((float)sumDatapoint.MaxScore) / ((float)datapoints.Length));
        int totalScore = (int)(((float)sumDatapoint.MaxScore) / ((float)datapoints.Length));

        bool churned = false;
        if (churnedVal > 0)
        {
            churned = true;
        }

        return new Datapoint("", hoursPlayed, level, pelletsEaten, fruitEaten, ghostsEaten, avgScore, maxScore, totalScore, churned);
    }

    //TODO; alter this distance function
    //WARNING: DO NOT USE CHURNED AS PART OF THIS FUNCTION
    public static float Distance(Datapoint a, Datapoint b)
    {
        float dist = 0;
        dist += Mathf.Abs(a.HoursPlayed - b.HoursPlayed)/2060f;
        dist += Mathf.Abs(a.Level - b.Level)/304f;
        dist += Mathf.Abs(a.PelletsEaten - b.PelletsEaten)/502005f;
        dist += Mathf.Abs(a.FruitEaten - b.FruitEaten)/1484f;
        dist += Mathf.Abs(a.GhostsEaten - b.GhostsEaten)/40942f;
        dist += Mathf.Abs(a.AvgScore - b.AvgScore)/1608f;
        dist += Mathf.Abs(a.MaxScore - b.MaxScore)/4809f;
        dist += Mathf.Abs(a.TotalScore - b.TotalScore)/274734f;
        return dist;
    }
    
}

