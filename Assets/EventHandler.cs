using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EventHandler : MonoBehaviour
{
    public GameObject Player;
    public GameObject ExplosionPrefab;
    public Camera FPSCam;
    public Rigidbody PlayerRB;
    public TextMeshProUGUI ChargingText;
    public ProgressBar ChargeBar;

    private bool ForcedShot = false;
    private float CHARGE_TIME = 2f;
    private float CurrentCharge = 0;
    private float ChargedPercent = 0;
    private float HoldingTime = 0;
    private float MAX_HOLDING_TIME = 3;
    private float EXPLOSION_STRENGTH = 1000f;
    private float EXPLOSION_RADIUS = 50f;
    private float DefTimeSinceLastFired = 1;
    private float TimeSinceLastFired = 1;
    private float SHOT_MAX_DISTANCE = 100;
    private float LastChargedPercent = 100;
    //[SerializeField] private float EXPLOSION_POWER = 100f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        WriteChargeLevelToTMP();
        TimeSinceLastFired -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetButton("Fire1"))
        {
            CurrentCharge += Time.deltaTime;
            if (CurrentCharge < CHARGE_TIME)
            {
                CurrentCharge = Mathf.Min(CurrentCharge += Time.deltaTime, CHARGE_TIME);

                

            }
            else// if (CurrentCharge >= CHARGE_TIME)
            {
                HoldingTime+= Time.deltaTime;
                if (HoldingTime >= MAX_HOLDING_TIME)
                {
                    AttemptFire();
                    ForcedShot = true;
                }
            }

        }
        if (Input.GetButtonUp("Fire1"))
        {
            AttemptFire();
            ForcedShot = false;
            LastChargedPercent = ChargedPercent;
            CurrentCharge= 0;
            HoldingTime = 0;
        }
    }


    bool CanFire()
    {
        if (TimeSinceLastFired < 0)
            return true;
        else return false;
    }

    void AttemptFire()
    {
        if (!CanFire()) { return; }
        if (ForcedShot)
        {
            ChargeBar.BarValue = 0;
            return; 
        }
        //Debug.Log("FIRE COMMAND SENT");
        RaycastHit hit;
        if (Physics.Raycast(FPSCam.transform.position, FPSCam.transform.forward, out hit, SHOT_MAX_DISTANCE))
        {
            //Debug.Log(hit.transform.name);

            //Debug.Log(GameObject.Find("Player").transform.position + " E: " + hit.transform.position);

            GameObject impact = Instantiate(ExplosionPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            impact.transform.localScale = new Vector3(ChargedPercent, ChargedPercent, ChargedPercent);
            if (Vector3.Distance(hit.point, PlayerRB.position) <=SHOT_MAX_DISTANCE*LastChargedPercent)
            {
                PlayerRB.velocity = new Vector3(PlayerRB.velocity.x, 0, PlayerRB.velocity.z);
                PlayerRB.AddExplosionForce((EXPLOSION_STRENGTH * ChargedPercent / 100), hit.point, EXPLOSION_RADIUS);
            }
            Destroy(impact, 2);
            TimeSinceLastFired = DefTimeSinceLastFired;
        }
    }
    void WriteChargeLevelToTMP()
    {
        if (ForcedShot)
        {
            ChargingText.text = "";
            return;
        }
        ChargedPercent = ((int)(CurrentCharge / CHARGE_TIME * 100));
        ChargeBar.BarValue = ChargedPercent;
        if (ChargedPercent < 99)
        {
            ChargingText.text=ChargedPercent.ToString() + "%";
        }
        else
        {
            ChargingText.text = "MAX CHARGE!";
        }
    }
}
