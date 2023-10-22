using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneTag : MonoBehaviour {
	
	[Space(3)]
	[Header("Status")]
	[Space(10)]
	
	public string   gene_string;
	
	public bool   doUpdateGenetics  = false;
	public bool   doInjectGenetics  = false;
	
	
	[Space(3)]
	[Header("Body transforms")]
	[Space(10)]
	
	// Offsets
	public Transform Body;
	public Transform Neck;
	public Transform LegLeft;
	public Transform LegRight;
	public Transform ArmLeft;
	public Transform ArmRight;
	// Models
	public Transform Torso;
	public Transform Head;
	public Transform LimbFrontLeft;
	public Transform LimbFrontRight;
	public Transform LimbRearLeft;
	public Transform LimbRearRight;
	
	
	
	//
	// Genetic sequence data
	//
	
	[Space(3)]
	[Header("Offset")]
	[Space(10)]
	
	public Vector3 BodyO;              // Body offset
	public Vector3 HeadO;              // Neck offset
	public Vector3 LimbFLO;            // Limb offset front left
	public Vector3 LimbFRO;            // Limb offset front right
	public Vector3 LimbRLO;            // Limb offset rear left
	public Vector3 LimbRRO;            // Limb offset rear right
	
	[Space(3)]
	[Header("Position")]
	[Space(10)]
	
	public Vector3 BodyP;              // Torso position
	public Vector3 HeadP;              // Head position
	public Vector3 LimbFLP;            // Limb position front left
	public Vector3 LimbFRP;            // Limb position front right
	public Vector3 LimbRLP;            // Limb position rear left
	public Vector3 LimbRRP;            // Limb position rear right
	
	[Space(3)]
	[Header("Rotation")]
	[Space(10)]
	
	public Vector3 BodyR;              // Body orientation
	public Vector3 HeadR;              // Head orientation
	public Vector3 LimbFLR;            // Limb front left orientation
	public Vector3 LimbFRR;            // Limb front right orientation
	public Vector3 LimbRLR;            // Limb rear left orientation
	public Vector3 LimbRRR;            // Limb rear right orientation
	
	[Space(3)]
	[Header("Scale")]
	[Space(10)]
	
	public Vector3 BodyS;              // Body scale
	public Vector3 HeadS;              // Head scale
	public Vector3 LimbFLS;            // Limb front left scale
	public Vector3 LimbFRS;            // Limb front right scale
	public Vector3 LimbRLS;            // Limb rear left scale
	public Vector3 LimbRRS;            // Limb rear right scale
	
	[Space(3)]
	[Header("Color")]
	[Space(10)]
	
	public Vector3 BodyC;              // Body color
	public Vector3 HeadC;              // Head color
	public Vector3 LimbFLC;            // Limb front left color
	public Vector3 LimbFRC;            // Limb front right color
	public Vector3 LimbRLC;            // Limb rear left color
	public Vector3 LimbRRC;            // Limb rear right color
	
	[Space(3)]
	[Header("Traits")]
	[Space(10)]
	
	public bool   masculine      = false;   // Is male(true) or female(false)
	public long   ageSpan        = 1000;    // Max average life span in ticks
	public int    ageWhenBorn    = -800;    // Age when entity is born
	public int    ageWhenAdult   = 0;       // Age when entity stops growing
	public float  ageSizeInc     = 1.0f;    // Size increment per tick until adult age
	public float  ageYouthScale  = 0.7f;    // Scale when born
	
	
	
	
	public void injectGeneticSequence(string geneString) {
        
        string[] genes = geneString.Split('|');
        string[] segments;
        
        // Offset position
        segments  = genes[0].Split(',');
        BodyO.x   = float.Parse(segments[0]); BodyO.y   = float.Parse(segments[1]); BodyO.z   = float.Parse(segments[2]);
        segments  = genes[1].Split(',');
        HeadO.x   = float.Parse(segments[0]); HeadO.y   = float.Parse(segments[1]); HeadO.z   = float.Parse(segments[2]);
        segments  = genes[2].Split(',');
        LimbFLO.x = float.Parse(segments[0]); LimbFLO.y = float.Parse(segments[1]); LimbFLO.z = float.Parse(segments[2]);
        segments  = genes[3].Split(',');
        LimbFRO.x = float.Parse(segments[0]); LimbFRO.y = float.Parse(segments[1]); LimbFRO.z = float.Parse(segments[2]);
        segments  = genes[4].Split(',');
        LimbRLO.x = float.Parse(segments[0]); LimbRLO.y = float.Parse(segments[1]); LimbRLO.z = float.Parse(segments[2]);
        segments  = genes[5].Split(',');
        LimbRRO.x = float.Parse(segments[0]); LimbRRO.y = float.Parse(segments[1]); LimbRRO.z = float.Parse(segments[2]);
        
        // Position
        segments  = genes[6].Split(',');
        BodyP.x   = float.Parse(segments[0]); BodyP.y   = float.Parse(segments[1]); BodyP.z   = float.Parse(segments[2]);
        segments  = genes[7].Split(',');
        HeadP.x   = float.Parse(segments[0]); HeadP.y   = float.Parse(segments[1]); HeadP.z   = float.Parse(segments[2]);
        segments  = genes[8].Split(',');
        LimbFLP.x = float.Parse(segments[0]); LimbFLP.y = float.Parse(segments[1]); LimbFLP.z = float.Parse(segments[2]);
        segments  = genes[9].Split(',');
        LimbFRP.x = float.Parse(segments[0]); LimbFRP.y = float.Parse(segments[1]); LimbFRP.z = float.Parse(segments[2]);
        segments  = genes[10].Split(',');
        LimbRLP.x = float.Parse(segments[0]); LimbRLP.y = float.Parse(segments[1]); LimbRLP.z = float.Parse(segments[2]);
        segments  = genes[11].Split(',');
        LimbRRP.x = float.Parse(segments[0]); LimbRRP.y = float.Parse(segments[1]); LimbRRP.z = float.Parse(segments[2]);
        
        // Orientation
        segments  = genes[12].Split(',');
        BodyR.x   = float.Parse(segments[0]); BodyR.y   = float.Parse(segments[1]); BodyR.z   = float.Parse(segments[2]);
        segments  = genes[13].Split(',');
        HeadR.x   = float.Parse(segments[0]); HeadR.y   = float.Parse(segments[1]); HeadR.z   = float.Parse(segments[2]);
        segments  = genes[14].Split(',');
        LimbFLR.x = float.Parse(segments[0]); LimbFLR.y = float.Parse(segments[1]); LimbFLR.z = float.Parse(segments[2]);
        segments  = genes[15].Split(',');
        LimbFRR.x = float.Parse(segments[0]); LimbFRR.y = float.Parse(segments[1]); LimbFRR.z = float.Parse(segments[2]);
        segments  = genes[16].Split(',');
        LimbRLR.x = float.Parse(segments[0]); LimbRLR.y = float.Parse(segments[1]); LimbRLR.z = float.Parse(segments[2]);
        segments  = genes[17].Split(',');
        LimbRRR.x = float.Parse(segments[0]); LimbRRR.y = float.Parse(segments[1]); LimbRRR.z = float.Parse(segments[2]);
        
        // Scale
        segments = genes[18].Split(',');
        BodyS.x   = float.Parse(segments[0]); BodyS.y   = float.Parse(segments[1]); BodyS.z   = float.Parse(segments[2]);
        segments = genes[19].Split(',');
        HeadS.x   = float.Parse(segments[0]); HeadS.y   = float.Parse(segments[1]); HeadS.z   = float.Parse(segments[2]);
        segments = genes[20].Split(',');
        LimbFLS.x = float.Parse(segments[0]); LimbFLS.y = float.Parse(segments[1]); LimbFLS.z = float.Parse(segments[2]);
        segments = genes[21].Split(',');
        LimbFRS.x = float.Parse(segments[0]); LimbFRS.y = float.Parse(segments[1]); LimbFRS.z = float.Parse(segments[2]);
        segments = genes[22].Split(',');
        LimbRLS.x = float.Parse(segments[0]); LimbRLS.y = float.Parse(segments[1]); LimbRLS.z = float.Parse(segments[2]);
        segments = genes[23].Split(',');
        LimbRRS.x = float.Parse(segments[0]); LimbRRS.y = float.Parse(segments[1]); LimbRRS.z = float.Parse(segments[2]);
        
        return;
    }
	
	
	
	
	
	public void extractGeneticSequence() {
        
        // Reset the genetic string
        gene_string = "";
        
        // Offset position
        gene_string += BodyO.x.ToString()+","+BodyO.y.ToString()+","+BodyO.z.ToString()+"|";
        gene_string += HeadO.x.ToString()+","+HeadO.y.ToString()+","+HeadO.z.ToString()+"|";
        gene_string += LimbFLO.x.ToString()+","+LimbFLO.y.ToString()+","+LimbFLO.z.ToString()+"|";
        gene_string += LimbFRO.x.ToString()+","+LimbFRO.y.ToString()+","+LimbFRO.z.ToString()+"|";
        gene_string += LimbRLO.x.ToString()+","+LimbRLO.y.ToString()+","+LimbRLO.z.ToString()+"|";
        gene_string += LimbRRO.x.ToString()+","+LimbRRO.y.ToString()+","+LimbRRO.z.ToString()+"|";
        
        // Position
        gene_string += BodyP.x.ToString()+","+BodyP.y.ToString()+","+BodyP.z.ToString()+"|";
        gene_string += HeadP.x.ToString()+","+HeadP.y.ToString()+","+HeadP.z.ToString()+"|";
        gene_string += LimbFLP.x.ToString()+","+LimbFLP.y.ToString()+","+LimbFLP.z.ToString()+"|";
        gene_string += LimbFRP.x.ToString()+","+LimbFRP.y.ToString()+","+LimbFRP.z.ToString()+"|";
        gene_string += LimbRLP.x.ToString()+","+LimbRLP.y.ToString()+","+LimbRLP.z.ToString()+"|";
        gene_string += LimbRRP.x.ToString()+","+LimbRRP.y.ToString()+","+LimbRRP.z.ToString()+"|";
        
        // Orientation
        gene_string += Quaternion.Euler(BodyR).x.ToString()+","+Quaternion.Euler(BodyR).y.ToString()+","+Quaternion.Euler(BodyR).z.ToString()+"|";
        gene_string += Quaternion.Euler(HeadR).x.ToString()+","+Quaternion.Euler(HeadR).y.ToString()+","+Quaternion.Euler(HeadR).z.ToString()+"|";
        gene_string += Quaternion.Euler(LimbFLR).x.ToString()+","+Quaternion.Euler(LimbFLR).y.ToString()+","+Quaternion.Euler(LimbFLR).z.ToString()+"|";
        gene_string += Quaternion.Euler(LimbFRR).x.ToString()+","+Quaternion.Euler(LimbFRR).y.ToString()+","+Quaternion.Euler(LimbFRR).z.ToString()+"|";
        gene_string += Quaternion.Euler(LimbRLR).x.ToString()+","+Quaternion.Euler(LimbRLR).y.ToString()+","+Quaternion.Euler(LimbRLR).z.ToString()+"|";
        gene_string += Quaternion.Euler(LimbRRR).x.ToString()+","+Quaternion.Euler(LimbRRR).y.ToString()+","+Quaternion.Euler(LimbRRR).z.ToString()+"|";
        
        // Scale
        gene_string += BodyS.x.ToString()+","+BodyS.y.ToString()+","+BodyS.z.ToString()+"|";
        gene_string += HeadS.x.ToString()+","+HeadS.y.ToString()+","+HeadS.z.ToString()+"|";
        gene_string += LimbFLS.x.ToString()+","+LimbFLS.y.ToString()+","+LimbFLS.z.ToString()+"|";
        gene_string += LimbFRS.x.ToString()+","+LimbFRS.y.ToString()+","+LimbFRS.z.ToString()+"|";
        gene_string += LimbRLS.x.ToString()+","+LimbRLS.y.ToString()+","+LimbRLS.z.ToString()+"|";
        gene_string += LimbRRS.x.ToString()+","+LimbRRS.y.ToString()+","+LimbRRS.z.ToString();
        
        return;
    }
	
	
	
    // Update physical characteristics
    public void updateGenetics() {
        
        Body.          localPosition  = BodyO;
        Neck.          localPosition  = HeadO;
        ArmLeft.       localPosition  = LimbFLO;
        ArmRight.      localPosition  = LimbFRO;
        LegLeft.       localPosition  = LimbRLO;
        LegRight.      localPosition  = LimbRRO;
        
        Torso.         localPosition  = BodyP;
        Head.          localPosition  = HeadP;
        LimbFrontLeft. localPosition  = LimbFLP;
        LimbFrontRight.localPosition  = LimbFRP;
        LimbRearLeft.  localPosition  = LimbRLP;
        LimbRearRight. localPosition  = LimbRRP;
        
        Torso.         localRotation  = Quaternion.Euler(BodyR);
        Head.          localRotation  = Quaternion.Euler(HeadR);
        ArmLeft.       localRotation  = Quaternion.Euler(LimbFLR);
        ArmRight.      localRotation  = Quaternion.Euler(LimbFRR);
        LegLeft.       localRotation  = Quaternion.Euler(LimbRLR);
        LegRight.      localRotation  = Quaternion.Euler(LimbRRR);
        
        Torso.         localScale = BodyS;
        Head.          localScale = HeadS;
        LimbFrontLeft. localScale = LimbFLS;
        LimbFrontRight.localScale = LimbFRS;
        LimbRearLeft.  localScale = LimbRLS;
        LimbRearRight. localScale = LimbRRS;
        
        for (int i=0; i < Torso.transform.parent.transform.childCount; i++) 
            Torso.transform.parent.transform.GetChild(i).GetComponent<Renderer>().material.color = new Color(BodyC.x, BodyC.y, BodyC.z, 1.0f);
        
        for (int i=0; i < Head.transform.parent.transform.childCount; i++) 
            Head.transform.parent.transform.GetChild(i).GetComponent<Renderer>().material.color = new Color(HeadC.x, HeadC.y, HeadC.z, 1.0f);
        
        for (int i=0; i < LimbFrontLeft.transform.parent.transform.childCount; i++) 
            LimbFrontLeft.transform.parent.transform.GetChild(i).GetComponent<Renderer>().material.color = new Color(LimbFLC.x, LimbFLC.y, LimbFLC.z, 1.0f);
        
        for (int i=0; i < LimbFrontRight.transform.parent.transform.childCount; i++) 
            LimbFrontRight.transform.parent.transform.GetChild(i).GetComponent<Renderer>().material.color = new Color(LimbFRC.x, LimbFRC.y, LimbFRC.z, 1.0f);
        
        for (int i=0; i < LimbRearLeft.transform.parent.transform.childCount; i++) 
            LimbRearLeft.transform.parent.transform.GetChild(i).GetComponent<Renderer>().material.color = new Color(LimbRLC.x, LimbRLC.y, LimbRLC.z, 1.0f);
        
        for (int i=0; i < LimbRearRight.transform.parent.transform.childCount; i++) 
            LimbRearRight.transform.parent.transform.GetChild(i).GetComponent<Renderer>().material.color = new Color(LimbRRC.x, LimbRRC.y, LimbRRC.z, 1.0f);
        
	}
	
	
	
	
	//
	// Add a mutation to a gene
	//
	
	public void addRandomMutation(int gene, float mutationFactor) {
        float mutationValue = Random.Range(0f, mutationFactor);
        
        // Offset gene
        if (gene == 0) {
            mutationValue = Random.Range(0f, mutationFactor);
            BodyO   += new Vector3(Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue));
            mutationValue = Random.Range(0f, mutationFactor);
            HeadO   += new Vector3(Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue));
            mutationValue = Random.Range(0f, mutationFactor);
            LimbFLO += new Vector3(Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue));
            mutationValue = Random.Range(0f, mutationFactor);
            LimbFRO += new Vector3(Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue));
            mutationValue = Random.Range(0f, mutationFactor);
            LimbRLO += new Vector3(Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue));
            mutationValue = Random.Range(0f, mutationFactor);
            LimbRRO += new Vector3(Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue));
        }
        
        // Position gene
        if (gene == 1) {
            mutationValue = Random.Range(0f, mutationFactor);
            BodyP   += new Vector3(Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue));
            mutationValue = Random.Range(0f, mutationFactor);
            HeadP   += new Vector3(Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue));
            mutationValue = Random.Range(0f, mutationFactor);
            LimbFLP += new Vector3(Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue));
            mutationValue = Random.Range(0f, mutationFactor);
            LimbFRP += new Vector3(Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue));
            mutationValue = Random.Range(0f, mutationFactor);
            LimbRLP += new Vector3(Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue));
            mutationValue = Random.Range(0f, mutationFactor);
            LimbRRP += new Vector3(Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue));
        }
        
        // Orientation gene
        if (gene == 2) {
            mutationValue = Random.Range(0f, mutationFactor);
            BodyR   += new Vector3(Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue));
            mutationValue = Random.Range(0f, mutationFactor);
            HeadR   += new Vector3(Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue));
            mutationValue = Random.Range(0f, mutationFactor);
            LimbFLR += new Vector3(Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue));
            mutationValue = Random.Range(0f, mutationFactor);
            LimbFRR += new Vector3(Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue));
            mutationValue = Random.Range(0f, mutationFactor);
            LimbRLR += new Vector3(Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue));
            mutationValue = Random.Range(0f, mutationFactor);
            LimbRRR += new Vector3(Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue));
        }
        
        // Scale gene
        if (gene == 3) {
            mutationValue = Random.Range(0f, mutationFactor);
            BodyS   += new Vector3(Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue));
            mutationValue = Random.Range(0f, mutationFactor);
            HeadS   += new Vector3(Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue));
            mutationValue = Random.Range(0f, mutationFactor);
            LimbFLS += new Vector3(Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue));
            mutationValue = Random.Range(0f, mutationFactor);
            LimbFRS += new Vector3(Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue));
            mutationValue = Random.Range(0f, mutationFactor);
            LimbRLS += new Vector3(Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue));
            mutationValue = Random.Range(0f, mutationFactor);
            LimbRRS += new Vector3(Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue));
        }
        
        // Color gene
        if (gene == 4) {
            mutationValue = Random.Range(0f, mutationFactor);
            BodyC   += new Vector3(Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue));
            mutationValue = Random.Range(0f, mutationFactor);
            HeadC   += new Vector3(Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue));
            mutationValue = Random.Range(0f, mutationFactor);
            LimbFLC += new Vector3(Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue));
            mutationValue = Random.Range(0f, mutationFactor);
            LimbFRC += new Vector3(Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue));
            mutationValue = Random.Range(0f, mutationFactor);
            LimbRLC += new Vector3(Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue));
            mutationValue = Random.Range(0f, mutationFactor);
            LimbRRC += new Vector3(Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue), Random.Range(0f, mutationValue) - Random.Range(0f, mutationValue));
        }
        
        return;
    }
    
	
	
	
	
	public void mergeGenetics(GeneTag m_Gene, GeneTag p_Gene) {
        
        // Initial chance for significant genetic change
        
        float bias = Random.Range(0f, 10f) * 0.1f;
        
        // Offset
        BodyO   = Vector3.Lerp(m_Gene.BodyO,   p_Gene.BodyO,   bias);
        HeadO   = Vector3.Lerp(m_Gene.HeadO,   p_Gene.HeadO,   bias);
        LimbFLO = Vector3.Lerp(m_Gene.LimbFLO, p_Gene.LimbFLO, bias);
        LimbFRO = Vector3.Lerp(m_Gene.LimbFRO, p_Gene.LimbFRO, bias);
        LimbRLO = Vector3.Lerp(m_Gene.LimbRLO, p_Gene.LimbRLO, bias);
        LimbRRO = Vector3.Lerp(m_Gene.LimbRRO, p_Gene.LimbRRO, bias);
        
        bias = Random.Range(0f, 10f) * 0.1f;
        
        // Position
        BodyP   = Vector3.Lerp(m_Gene.BodyP,   p_Gene.BodyP,   bias);
        HeadP   = Vector3.Lerp(m_Gene.HeadP,   p_Gene.HeadP,   bias);
        LimbFLP = Vector3.Lerp(m_Gene.LimbFLP, p_Gene.LimbFLP, bias);
        LimbFRP = Vector3.Lerp(m_Gene.LimbFRP, p_Gene.LimbFRP, bias);
        LimbRLP = Vector3.Lerp(m_Gene.LimbRLP, p_Gene.LimbRLP, bias);
        LimbRRP = Vector3.Lerp(m_Gene.LimbRRP, p_Gene.LimbRRP, bias);
        
        bias = Random.Range(0f, 10f) * 0.1f;
        
        // Rotation
        BodyR   = Vector3.Lerp(m_Gene.BodyR,   p_Gene.BodyR,   bias);
        HeadR   = Vector3.Lerp(m_Gene.HeadR,   p_Gene.HeadR,   bias);
        LimbFLR = Vector3.Lerp(m_Gene.LimbFLR, p_Gene.LimbFLR, bias);
        LimbFRR = Vector3.Lerp(m_Gene.LimbFRR, p_Gene.LimbFRR, bias);
        LimbRLR = Vector3.Lerp(m_Gene.LimbRLR, p_Gene.LimbRLR, bias);
        LimbRRR = Vector3.Lerp(m_Gene.LimbRRR, p_Gene.LimbRRR, bias);
        
        bias = Random.Range(0f, 10f) * 0.1f;
        
        // Scale
        BodyS   = Vector3.Lerp(m_Gene.BodyS,   p_Gene.BodyS,   bias);
        HeadS   = Vector3.Lerp(m_Gene.HeadS,   p_Gene.HeadS,   bias);
        LimbFLS = Vector3.Lerp(m_Gene.LimbFLS, p_Gene.LimbFLS, bias);
        LimbFRS = Vector3.Lerp(m_Gene.LimbFRS, p_Gene.LimbFRS, bias);
        LimbRLS = Vector3.Lerp(m_Gene.LimbRLS, p_Gene.LimbRLS, bias);
        LimbRRS = Vector3.Lerp(m_Gene.LimbRRS, p_Gene.LimbRRS, bias);
        
        bias = Random.Range(0f, 10f) * 0.1f;
        
        // Color
        BodyC   = Vector3.Lerp(m_Gene.BodyC,   p_Gene.BodyC,   bias);
        HeadC   = Vector3.Lerp(m_Gene.HeadC,   p_Gene.HeadC,   bias);
        LimbFLC = Vector3.Lerp(m_Gene.LimbFLC, p_Gene.LimbFLC, bias);
        LimbFRC = Vector3.Lerp(m_Gene.LimbFRC, p_Gene.LimbFRC, bias);
        LimbRLC = Vector3.Lerp(m_Gene.LimbRLC, p_Gene.LimbRLC, bias);
        LimbRRC = Vector3.Lerp(m_Gene.LimbRRC, p_Gene.LimbRRC, bias);
        
        // Traits
        
        masculine = false;
        if (Random.Range(0, 10) > 4) 
            masculine = true;
        
        ageSpan = m_Gene.ageSpan;
        if (Random.Range(0, 10) > 4) 
            ageSpan = p_Gene.ageSpan;
        
        ageWhenBorn    = (int)Mathf.Lerp((float)m_Gene.ageWhenBorn,  (float)p_Gene.ageWhenBorn, bias);
        ageWhenAdult   = (int)Mathf.Lerp((float)m_Gene.ageWhenAdult, (float)p_Gene.ageWhenAdult, bias);
        ageSizeInc     = Mathf.Lerp(m_Gene.ageSizeInc, p_Gene.ageSizeInc, bias);
        ageYouthScale  = Mathf.Lerp(m_Gene.ageYouthScale, p_Gene.ageYouthScale, bias);
        
        return;
	}
	
}
