namespace WaterState;

public class Water
{
    public double Temperature;
    public int Amount;
    public WaterState State { get; private set; }
    public double ProportionFirstState;
    public bool isMeltedOrGas;


    public Water(int amount, int temperature, double? proportion = null)
    {

        Amount = amount;
        Temperature = temperature;
        State = GetWaterState(temperature, proportion, isMeltedOrGas);

        if (proportion.HasValue)
        {
            ProportionFirstState = proportion.Value;
        }
    }

    private WaterState GetWaterState(double temperature, double? proportion, bool isMeltedOrGas)
    {
        return (temperature is 100 or 0) && !proportion.HasValue ? ProportionIsNull() :
            (temperature is 100 or 0) && proportion.HasValue && !isMeltedOrGas ? FluidGasOrIceFluid() :
            (temperature < 100 && temperature > 0 || temperature >= 0 && isMeltedOrGas) ? WaterState.Fluid :
            temperature > 100 ? WaterState.Gas : WaterState.Ice;
    }

    private WaterState FluidGasOrIceFluid()
    {
        if (Temperature > 0)
        {
            return WaterState.FluidAndGas;
        }
        return WaterState.IceAndFluid;
    }

    public void AddEnergy(double energyAmount)
    {
        if (Temperature > 0)
        {
            Temperature += energyAmount / Amount;
        }
        else
        {
            /* heatTo0 - mengden energi som brukes til å få  */
            var energyToHeatIce = Temperature * Amount;
            var energyToMeltIce = Amount * 80;
            energyAmount -= energyToHeatIce;
            
            if (energyAmount >= energyToHeatIce + energyToMeltIce)
            {
                energyAmount -= energyToHeatIce - energyToMeltIce;
                var proportionHeatedWater = energyAmount / ((100 - 0) * Amount * 1);
                Temperature = 0 + proportionHeatedWater * 100;
                //Temperature = 0;
                isMeltedOrGas = true;
            }

            else
            {
                ProportionFirstState = 1 - energyAmount / energyToMeltIce;
                Temperature += ProportionFirstState;
                isMeltedOrGas = false;
            }
        }

        State = GetWaterState(Temperature, ProportionFirstState, isMeltedOrGas);
    }

    private static WaterState ProportionIsNull()
    {
        throw new ArgumentException("When temperature is 0 or 100, you must provide a value for proportion");
    }

    public enum WaterState
    {
        Fluid,
        Ice,
        Gas,
        FluidAndGas,
        IceAndFluid,
    }
}