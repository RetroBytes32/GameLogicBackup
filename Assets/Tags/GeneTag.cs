using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneTag : MonoBehaviour {
	
	[Space(3)]
	[Header("Body transforms")]
	[Space(10)]
	
	public string   gene_string;
	
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
	// Genetic data
	
	public float AdultSizeMul;         // Adult body size multiplier
	
	
	// Body position offsets
	
	[Space(3)]
	[Header("Offset")]
	[Space(10)]
	
	public Vector3 BodyO;              // Body offset
	public Vector3 HeadO;              // Neck offset
	public Vector3 LimbFLO;            // Limb offset front left
	public Vector3 LimbFRO;            // Limb offset front right
	public Vector3 LimbRLO;            // Limb offset rear left
	public Vector3 LimbRRO;            // Limb offset rear right
	
	// Body position offsets
	
	[Space(3)]
	[Header("Position")]
	[Space(10)]
	
	public Vector3 BodyP;              // Torso position
	public Vector3 HeadP;              // Head position
	public Vector3 LimbFLP;            // Limb position front left
	public Vector3 LimbFRP;            // Limb position front right
	public Vector3 LimbRLP;            // Limb position rear left
	public Vector3 LimbRRP;            // Limb position rear right
	
	// Body rotation values
	
	[Space(3)]
	[Header("Rotation")]
	[Space(10)]
	
	public Vector3 BodyR;              // Body orientation
	public Vector3 HeadR;              // Head orientation
	public Vector3 LimbFLR;            // Limb front left orientation
	public Vector3 LimbFRR;            // Limb front right orientation
	public Vector3 LimbRLR;            // Limb rear left orientation
	public Vector3 LimbRRR;            // Limb rear right orientation
	
	// Body scale factors
	
	[Space(3)]
	[Header("Scale")]
	[Space(10)]
	
	public Vector3 BodyS;              // Body scale
	public Vector3 HeadS;              // Head scale
	public Vector3 LimbFLS;            // Limb front left scale
	public Vector3 LimbFRS;            // Limb front right scale
	public Vector3 LimbRLS;            // Limb rear left scale
	public Vector3 LimbRRS;            // Limb rear right scale
	
	// Body color varients
	
	[Space(3)]
	[Header("Color")]
	[Space(10)]
	
	public Vector3 BodyC;              // Body color
	public Vector3 HeadC;              // Head color
	public Vector3 LimbFLC;            // Limb front left color
	public Vector3 LimbFRC;            // Limb front right color
	public Vector3 LimbRLC;            // Limb rear left color
	public Vector3 LimbRRC;            // Limb rear right color
	
	
	
	
	
	public void updateGenetics() {
        
        //ChunkGeneration generator = this.transform.parent.transform.parent.transform.parent.transform.parent.transform.GetChild(0).gameObject.GetComponent<ChunkGeneration>();
        
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
        
        for (int i=0; i < LimbFrontLeft.transform.parent.transform.childCount; i++) 
            LimbFrontRight.transform.parent.transform.GetChild(i).GetComponent<Renderer>().material.color = new Color(LimbFRC.x, LimbFRC.y, LimbFRC.z, 1.0f);
        
        for (int i=0; i < LimbFrontLeft.transform.parent.transform.childCount; i++) 
            LimbRearLeft.transform.parent.transform.GetChild(i).GetComponent<Renderer>().material.color = new Color(LimbRLC.x, LimbRLC.y, LimbRLC.z, 1.0f);
        
        for (int i=0; i < LimbFrontLeft.transform.parent.transform.childCount; i++) 
            LimbRearRight.transform.parent.transform.GetChild(i).GetComponent<Renderer>().material.color = new Color(LimbRRC.x, LimbRRC.y, LimbRRC.z, 1.0f);
        
	}
	
	
	
	
	
	public void mergeGenetics(GeneTag m_Gene, GeneTag p_Gene, float bias) {
        
        float tolerance = 0.3f;
        
        // Initial chance for significant genetic change
        float biasTotal = bias;
        
        biasTotal = bias + (Random.Range(0.0f, tolerance) - Random.Range(0.0f, tolerance));
        
        // Offset
        BodyO   = Vector3.Lerp(m_Gene.BodyO,   p_Gene.BodyO,   biasTotal);
        HeadO   = Vector3.Lerp(m_Gene.HeadO,   p_Gene.HeadO,   biasTotal);
        LimbFLO = Vector3.Lerp(m_Gene.LimbFLO, p_Gene.LimbFLO, biasTotal);
        LimbFRO = Vector3.Lerp(m_Gene.LimbFRO, p_Gene.LimbFRO, biasTotal);
        LimbRLO = Vector3.Lerp(m_Gene.LimbRLO, p_Gene.LimbRLO, biasTotal);
        LimbRRO = Vector3.Lerp(m_Gene.LimbRRO, p_Gene.LimbRRO, biasTotal);
        
        biasTotal = bias + (Random.Range(0.0f, tolerance) - Random.Range(0.0f, tolerance));
        
        // Position
        BodyP   = Vector3.Lerp(m_Gene.BodyP,   p_Gene.BodyP,   biasTotal);
        HeadP   = Vector3.Lerp(m_Gene.HeadP,   p_Gene.HeadP,   biasTotal);
        LimbFLP = Vector3.Lerp(m_Gene.LimbFLP, p_Gene.LimbFLP, biasTotal);
        LimbFRP = Vector3.Lerp(m_Gene.LimbFRP, p_Gene.LimbFRP, biasTotal);
        LimbRLP = Vector3.Lerp(m_Gene.LimbRLP, p_Gene.LimbRLP, biasTotal);
        LimbRRP = Vector3.Lerp(m_Gene.LimbRRP, p_Gene.LimbRRP, biasTotal);
        
        biasTotal = bias + (Random.Range(0.0f, tolerance) - Random.Range(0.0f, tolerance));
        
        // Rotation
        BodyR   = Vector3.Lerp(m_Gene.BodyR,   p_Gene.BodyR,   biasTotal);
        HeadR   = Vector3.Lerp(m_Gene.HeadR,   p_Gene.HeadR,   biasTotal);
        LimbFLR = Vector3.Lerp(m_Gene.LimbFLR, p_Gene.LimbFLR, biasTotal);
        LimbFRR = Vector3.Lerp(m_Gene.LimbFRR, p_Gene.LimbFRR, biasTotal);
        LimbRLR = Vector3.Lerp(m_Gene.LimbRLR, p_Gene.LimbRLR, biasTotal);
        LimbRRR = Vector3.Lerp(m_Gene.LimbRRR, p_Gene.LimbRRR, biasTotal);
        
        biasTotal = bias + (Random.Range(0.0f, tolerance) - Random.Range(0.0f, tolerance));
        
        // Scale
        BodyS   = Vector3.Lerp(m_Gene.BodyS,   p_Gene.BodyS,   biasTotal);
        HeadS   = Vector3.Lerp(m_Gene.HeadS,   p_Gene.HeadS,   biasTotal);
        LimbFLS = Vector3.Lerp(m_Gene.LimbFLS, p_Gene.LimbFLS, biasTotal);
        LimbFRS = Vector3.Lerp(m_Gene.LimbFRS, p_Gene.LimbFRS, biasTotal);
        LimbRLS = Vector3.Lerp(m_Gene.LimbRLS, p_Gene.LimbRLS, biasTotal);
        LimbRRS = Vector3.Lerp(m_Gene.LimbRRS, p_Gene.LimbRRS, biasTotal);
        
        biasTotal = bias + (Random.Range(0.0f, tolerance) - Random.Range(0.0f, tolerance));
        
        // Color
        BodyC   = Vector3.Lerp(m_Gene.BodyC,   p_Gene.BodyC,   biasTotal);
        HeadC   = Vector3.Lerp(m_Gene.HeadC,   p_Gene.HeadC,   biasTotal);
        LimbFLC = Vector3.Lerp(m_Gene.LimbFLC, p_Gene.LimbFLC, biasTotal);
        LimbFRC = Vector3.Lerp(m_Gene.LimbFRC, p_Gene.LimbFRC, biasTotal);
        LimbRLC = Vector3.Lerp(m_Gene.LimbRLC, p_Gene.LimbRLC, biasTotal);
        LimbRRC = Vector3.Lerp(m_Gene.LimbRRC, p_Gene.LimbRRC, biasTotal);
        
        return;
	}
	
}
