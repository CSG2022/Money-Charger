using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#region CableMaterial Struct
public struct CableMaterial
{
    public float velicina;
    public float debljina;
    public float fill;

    public CableMaterial(float velicina, float debljina, float fill)
    {
        this.velicina = velicina;
        this.debljina = debljina;
        this.fill = fill;
    }
}
#endregion

public class ClickerBehaviour : MonoBehaviour
{
    #region Variables
    [SerializeField] float _decayPulseInterval = .5f;
    [SerializeField] float _stepPromeneVelicine = 10f;
    [SerializeField] float _stepPromeneDebljine = 10f;
    [SerializeField] float _stepPromeneFilla = 2f;


    [SerializeField] public Mesh Lvl1SwitchMesh;
    [SerializeField] public Mesh Lvl2SwitchMesh;
    [SerializeField] public Mesh Lvl3SwitchMesh;

    public IEnumerator DecayCoroutineHolder;
    public bool CoroutineIsExecuting;
    #endregion

    #region Start and FixedUpdate
    private void Start()
    {
        DecayCoroutineHolder = SlowlyDecayCablePower();
    }

    private void FixedUpdate()
    {
        if(GameManager.CablePowerMaterial.GetFloat("fill") > 0 && !CoroutineIsExecuting)
        {
            StartCoroutine(SlowlyDecayCablePower());
        }
    }
    #endregion

    #region Increment Money and Fill Battery
    public void IncrementByAmount(float amount)
    {
        IncreaseCablePower();
        float tmp = GameManager.BatteryFill.GetFloat("Vector1_bf3afcb57e66442ebfa625df53b0c77e") + amount;
        GameManager.BatteryFill.SetFloat("Vector1_bf3afcb57e66442ebfa625df53b0c77e", tmp);
        GameManager.CashAmount += amount;
    }
    #endregion

    #region Upgrades on passive income
    public void UpgradePassiveIncomeAmount(int amount)
    {
        GameManager.CashPerSecond += amount;
    }
    public void DowngradePassiveIncomeAmount(int amount)
    {
        if(GameManager.CashAmount - amount >= 0)
            GameManager.CashPerSecond -= amount;
        else
            GameManager.CashAmount = 0;
    }
    public void UpgradePassiveIncomeSpeed(float speedIncrease)
    {
        if(GameManager.PassiveIncomeDelayInSeconds > 0)
        {
        GameManager.PassiveIncomeDelayInSeconds += speedIncrease;
        if (GameManager.PassiveIncomeDelayInSeconds < 0)
            GameManager.PassiveIncomeDelayInSeconds = 0;
        }
    }
    public void DowngradePassiveIncomeSpeed(float speedIncrease)
    {
        if(GameManager.PassiveIncomeDelayInSeconds < 10)
        {
            GameManager.PassiveIncomeDelayInSeconds -= speedIncrease;
            if (GameManager.PassiveIncomeDelayInSeconds > 10)
                GameManager.PassiveIncomeDelayInSeconds = 10;
        }
    }
    #endregion

    #region CablePowerLogic

    public void IncreaseCablePower()
    {
        CableMaterial cableMaterialValues = GetCableMaterialValues();
        // check for maxValues!
        if (GameManager.CablePowerMaterial.GetFloat("velicina") >= 100)
        {
            GameManager.CablePowerMaterial.SetFloat("velicina", 100);
        }
        else
        {
            GameManager.CablePowerMaterial.SetFloat("velicina", cableMaterialValues.velicina + _stepPromeneVelicine);
        }
        if (GameManager.CablePowerMaterial.GetFloat("debljina") >= 100)
        {
            GameManager.CablePowerMaterial.SetFloat("debljina", 100);
        }
        else
        {
            GameManager.CablePowerMaterial.SetFloat("debljina", cableMaterialValues.debljina + _stepPromeneDebljine);
        }
        if (GameManager.CablePowerMaterial.GetFloat("fill") >= 20)
        {
            GameManager.CablePowerMaterial.SetFloat("fill", 20);
        }
        else
        {
            GameManager.CablePowerMaterial.SetFloat("fill", cableMaterialValues.fill + _stepPromeneFilla);
        }
        // Turn all these into Ternary Operators!

        if(!CoroutineIsExecuting)
        {
            StartCoroutine(DecayCoroutineHolder);
        }
    }

    IEnumerator SlowlyDecayCablePower()
    {
        GameManager.CableFillingSound.Play();
        CoroutineIsExecuting = true;
        while(true)
        {
            if (!CoroutineIsExecuting)
            {
                StopCoroutine(DecayCoroutineHolder);
                break;
            }
            yield return new WaitForSeconds(_decayPulseInterval);

            CableMaterial cableMaterialValues = GetCableMaterialValues();
            if(cableMaterialValues.debljina > 0)
            {
                GameManager.CablePowerMaterial.SetFloat("debljina", cableMaterialValues.debljina -= _stepPromeneDebljine);
            }
            else
            {
                GameManager.CablePowerMaterial.SetFloat("debljina", 0);
            }
            if(cableMaterialValues.velicina > 0)
            {
                GameManager.CablePowerMaterial.SetFloat("velicina", cableMaterialValues.velicina -= _stepPromeneVelicine);
            }
            else
            {
                GameManager.CablePowerMaterial.SetFloat("velicina", 0);
            }
            if(cableMaterialValues.fill > 0)
            {
                GameManager.CablePowerMaterial.SetFloat("fill", cableMaterialValues.fill -= _stepPromeneFilla);
            }
            else
            {
                GameManager.CableFillingSound.Stop();
                CoroutineIsExecuting = false;
                break;
            }
           /* if(GameManager.CablePowerMaterial.GetFloat("fill") < 14) // variable for this number fillAlmostFullyCharged or similar
                GameManager.CablePowerMaterial.SetColor("Color_5301bdf4cff645039b3a8d78044deeae", Color.blue);
            else
                GameManager.CablePowerMaterial.SetColor("Color_5301bdf4cff645039b3a8d78044deeae", Color.yellow);*/

            CableMaterial cableMaterialValuesAfter = GetCableMaterialValues();
        }
        yield return null;
    }

    public void ResetElectricImpulse()
    {
        StopAllCoroutines();
        CoroutineIsExecuting = false;
        GameManager.CableFillingSound.Stop();
        GameManager.CablePowerMaterial.SetFloat("velicina", 0);
        GameManager.CablePowerMaterial.SetFloat("debljina", 0);
        GameManager.CablePowerMaterial.SetFloat("fill", 0);
    }

    public CableMaterial GetCableMaterialValues()
    {
        var velicina = GameManager.CablePowerMaterial.GetFloat("velicina");
        var debljina = GameManager.CablePowerMaterial.GetFloat("debljina");
        var fill = GameManager.CablePowerMaterial.GetFloat("fill");
        return new CableMaterial(velicina, debljina, fill);
    }
    #endregion
}
