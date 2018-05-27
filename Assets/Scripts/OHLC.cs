using System;
using System.Globalization;

public class OHLC  {

	public float open;
	public float high;
	public float low;
	public float close;
	public DateTime time;
	public int volume;
	private string x;
	
	public OHLC(string[] ohlcStr) {
		time = Convert.ToDateTime (ohlcStr [0]);
		open = float.Parse (ohlcStr [1], CultureInfo.InvariantCulture.NumberFormat);
		high = float.Parse (ohlcStr [2], CultureInfo.InvariantCulture.NumberFormat);
		low = float.Parse (ohlcStr [3], CultureInfo.InvariantCulture.NumberFormat);
		close = float.Parse (ohlcStr [4], CultureInfo.InvariantCulture.NumberFormat);
		x = ohlcStr [5];
		volume = int.Parse (ohlcStr[6]);
	}

	override public string ToString() {
		return "Data = " + time.ToString() 
		+ "\nOpen = " + open.ToString() +
		"\nHigh = " + high.ToString() +
		"\nLow = " + low.ToString() +
		"\nClose = " + close.ToString() +
		"\nVolume = " + volume.ToString();
	}

}
