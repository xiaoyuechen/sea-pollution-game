﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PolluterAttrib : System.ICloneable
{
    public string title = "Default Polluter";
    public EconomicAttrib economicAttrib = new EconomicAttrib { };
    public PollutionAttrib pollutionAttrib = new PollutionAttrib { };
    public VulnerabilityAttrib vulnerabilityAttrib = new VulnerabilityAttrib { };
    public PlacementAttrib placementAttrib = new PlacementAttrib { };

    public PolluterAttrib() { }

    public PolluterAttrib(PolluterAttrib other)
    {
        title = (string)other.title.Clone();
        economicAttrib = (EconomicAttrib)other.economicAttrib.Clone();
        pollutionAttrib = (PollutionAttrib)other.pollutionAttrib.Clone();
        vulnerabilityAttrib = (VulnerabilityAttrib)other.vulnerabilityAttrib.Clone();
    }

    public object Clone()
    {
        return new PolluterAttrib(this);
    }

    public virtual string GetDescription()
    {
        return title + ":\n" +
               economicAttrib.GetDiscription() + pollutionAttrib.GetDiscription() + vulnerabilityAttrib.GetDescription();
    }
}

