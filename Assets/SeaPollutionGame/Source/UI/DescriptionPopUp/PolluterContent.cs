﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PolluterContent : PopUpPieChartContent
{
    public GameObject objectPieChart = null;

    public WorldWindow worldWindow { get; set; }

    [SerializeField]
    private TextMeshProUGUI textTitle = null;
    [SerializeField]
    private TextMeshProUGUI pieChartTitle = null;
    [SerializeField]
    private TextMeshProUGUI maxPieText = null;
    [SerializeField]
    private TextMeshProUGUI textEconomic = null;
    

    [Header("Vulnerabilities")]
    [SerializeField]
    private GameObject objectVulnerabilities = null;
    [SerializeField]
    private TextMeshProUGUI textVulnerabilities = null;
    [SerializeField]
    private bool showVulnerabilities = false;

    [Header("Resources")]
    [SerializeField]
    private GameObject objectResources = null;
    [SerializeField]
    private TextMeshProUGUI textResources = null;

    [Header("Recycles")]
    [SerializeField]
    private GameObject objectRecyclers = null;
    [SerializeField]
    private TextMeshProUGUI textRecyclers = null;

    private bool purchaseCheck = false;
    private bool iconCheck = false;

    public bool CheckGraphicPolluter(PurchasableIcon purchasableIcon)
    {
        PolluterAttrib attrib = purchasableIcon.GetPolluterAttributes();
        bool check = CheckPolluter(attrib);

        purchaseCheck = check;
        
        return check;
    }

    public bool CheckGraphicPolluter(PolluterIcon polluterIcon)
    {
        PolluterAttrib attrib = polluterIcon.GetPolluterAttributes();
        bool check = CheckPolluter(attrib);

        iconCheck = check;

        return check;
    }

    public bool CheckPolluter(Polluter polluter)
    {
        if(polluter != null)
        {
            imageIsDisaster = !polluter.IsAlive();
            
            bool check = CheckPolluter(polluter.GetAttrib());

            // special case in game
            PollutionMap map = new PollutionMap(polluter.GetPollutionMap());

            if (Util.SumMap(map) == 0)
            {
                objectPieChart.SetActive(false);
            }
            else
            {
                objectPieChart.SetActive(true);
            }

            if (Util.SumMap(map) < 0)
            {
                map = Util.MultiplyMap(map, (-1));
            }

            if(polluter is Filter)
            {
                pieChartTitle.text = "Reducing:";
                maxPieText.text = "/" + Mathf.Abs(Util.SumMap(new PollutionMap(polluter.GetAttrib().pollutionAttrib.emissions)));
            }

            if(polluter is Factory)
            {
                pieChartTitle.text = "Emitting:";
                maxPieText.text = "";
            }

            if(polluter is Recycler)
            {
                pieChartTitle.text = "Converting:";
                maxPieText.text = "";

                if(polluter.GetAttrib().recycleAttrib.conversions != null)
                {
                    foreach (RecycleAttrib.Conversion conversion in polluter.GetAttrib().recycleAttrib.conversions)
                    {
                        maxPieText.text += "/" + conversion.maxConversion.ToString() + "\n";
                    }
                }
            }

            SetPieChart(pieChart, map);

            return check;
        }

        return false;
    }

    public bool CheckPolluter(PolluterAttrib polluterAttrib)
    {
        bool hasFoundData = false;

        if (polluterAttrib != null)
        {
            hasFoundData = true;
            
            textTitle.text = polluterAttrib.title;

            textEconomic.text = "Price: " + polluterAttrib.economicAttrib.price + " Income: " + polluterAttrib.economicAttrib.profitPerTurn + "\nRemoval cost: " + polluterAttrib.economicAttrib.removalCost;

            PollutionMap map = new PollutionMap(polluterAttrib.pollutionAttrib.emissions);
            
            if(Util.SumMap(map) == 0)
            {
                objectPieChart.SetActive(false);
            } else
            {
                objectPieChart.SetActive(true);
            }

            if (Util.SumMap(map) < 0)
            {
                map = Util.MultiplyMap(map, (-1));
                pieChartTitle.text = "Reduces:";
            } else
            {
                pieChartTitle.text = "Emits:";
            }

            maxPieText.text = "";

            SetPieChart(pieChart, map);

            if (showVulnerabilities)
            {
                VulnerabilityAttrib vulnerabilityAttrib = polluterAttrib.vulnerabilityAttrib;
                if (vulnerabilityAttrib != null)
                {
                    if (vulnerabilityAttrib.vulnerabilities != null)
                    {
                        objectVulnerabilities.SetActive(true);

                        string vulnerabilityString = "Vulnerable to ";
                        foreach (VulnerabilityAttrib.Vulnerability v in vulnerabilityAttrib.vulnerabilities)
                        {
                            vulnerabilityString += v.disasterName + ":" + v.factor + " ";
                        }
                        textVulnerabilities.text = vulnerabilityString;
                    }
                    else
                    {
                        objectVulnerabilities.SetActive(false);

                        textVulnerabilities.text = "";
                    }
                }
            } else
            {
                objectVulnerabilities.SetActive(false);
            }

            ResourceAttrib resourceAttrib = polluterAttrib.resourceAttrib;
            if(resourceAttrib != null)
            {
                if(resourceAttrib.products != null)
                {
                    objectResources.SetActive(true);

                    textResources.text = "";

                    foreach (ResourceAttrib.Product product in resourceAttrib.products)
                    {
                        textResources.text += "Produce: " + product.productPerTurn + " of " + product.resourceName + " per turn\n";
                    }

                } else
                {
                    objectResources.SetActive(false);
                }
            }

            RecycleAttrib recycleAttrib = polluterAttrib.recycleAttrib;
            if (recycleAttrib != null)
            {
                if (recycleAttrib.conversions != null)
                {
                    pieChartTitle.text = "Converts:";

                    objectRecyclers.SetActive(true);

                    textRecyclers.text = "";

                    PollutionMap mapRecycle = new PollutionMap();

                    foreach(RecycleAttrib.Conversion conversion in recycleAttrib.conversions)
                    {
                        mapRecycle.Add(conversion.pollutantName, conversion.maxConversion);

                        textRecyclers.text += "" + conversion.pollutantName + " to " + conversion.convertTo + "\n"
                            + "Rate: " + conversion.conversionRate + "\n"; //+ " Max: " + conversion.maxConversion + "\n";
                    }

                    if(Util.SumMap(mapRecycle) != 0)
                    {
                        objectPieChart.SetActive(true);
                        SetPieChart(pieChart, mapRecycle);
                    }
                }
                else
                {
                    objectRecyclers.SetActive(false);
                }
            }

            VisualAttrib visualAttrib = polluterAttrib.visualAttrib;
            if (visualAttrib.imageName != "")
            {
                worldWindow.imageLoader.LoadImage(visualAttrib.imageName);
                imageToShow = true;
            }
            else
            {
                imageToShow = false;
            }
        }

        return hasFoundData;
    }
}
