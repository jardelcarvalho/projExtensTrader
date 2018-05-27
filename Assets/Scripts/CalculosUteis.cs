using System.Collections;
using System.Collections.Generic;

public class CalculosUteis {

	public float alfaHL;
	public float alfaVOL;
	public float deltaHL;
	public int deltaVOL;
	public float maiorH;
	public float menorL;
	public int maiorVOL;
	public float espessura;
	public float closeAtual;

	public float espacoCandles;
	public float espacoVolumes;
	public float espacoHorizontal;

	public float ESPACO = 0.07f;
	private const float ESPACO_ENTRE_VOLUMES_CANDLES = 0.07f;

	public CalculosUteis(UnityEngine.Vector3 vetor) { //Vetor é as dimensões da caixa de exibição do gráfico
		espacoCandles = vetor.y * 5 / 6 - ESPACO_ENTRE_VOLUMES_CANDLES; 
		espacoVolumes = vetor.y * 1 / 6;
		espacoHorizontal = vetor.x;
	}

	public void determinaUteis(OHLC[] arr, bool espacoCaixa) {
		maiorH = float.MinValue;
		menorL = float.MaxValue;
		maiorVOL = int.MinValue;
		foreach(OHLC elem in arr) {
			if(elem.high > maiorH) {
				maiorH = elem.high;
			}
			if(elem.low < menorL) {
				menorL = elem.low;
			}
			if(elem.volume > maiorVOL) {
				maiorVOL = elem.volume;
			}
			closeAtual = elem.close;
		}
		deltaHL = maiorH - menorL;
		deltaVOL = maiorVOL;
		if(deltaHL == .0f) {
			deltaHL = 0.00000001f;
		}
		alfaHL = espacoCandles / deltaHL;
		alfaVOL = espacoVolumes / deltaVOL;
		float decr = 0f;
		if(espacoCaixa) decr = 1;
		espessura = (espacoHorizontal - decr - ESPACO * (arr.Length - 1)) / (float) arr.Length;
	}	

	override public string ToString() {
		return "AlfaHL = " + alfaHL +
		"\nAlfaVOL = " + alfaVOL +
		"\nDeltaHL = " + deltaHL +
		"\nDeltaVol = " + deltaVOL +
		"\nMaiorH = " + maiorH +
		"\nMenorL = " + menorL +
		"\nMaiorVOL = " + maiorVOL;
	}

}
