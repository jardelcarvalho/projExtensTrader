using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraficoDeVelas : MonoBehaviour {

	private class FracaoGrafico {
		public GameObject hl, oc, vol;
	}

	[SerializeField] GameObject quad;
	private static Transform telaExibicao;
	private Color corAlta = new Color(0f, 1f, 0f);
	private Color corBaixa = new Color(1f, 0f, 0f);
	private Color corVolume = new Color(0f, 0f, 1f, 1f);
	private const float ESPESSURA_HL = 0.03f;
	public const float ESPACO_ENTRE_VOLUMES_CANDLES = 0.07f;
	private List<FracaoGrafico> lista = new List<FracaoGrafico>();
	
	public static void setTransformTelaExibicao(Transform t) {
		telaExibicao = t;
	}

	public void setCor(Color candleAlta, Color candleBaixa, Color volume) {
		corAlta = candleAlta;
		corBaixa = candleBaixa;
		corVolume = volume;
	}

	public IEnumerator desenharGraficoVelas(OHLC[] arr, CalculosUteis uteis) {
		float hl, oc;
		int vol;
		bool alta;
		Color cor;
		//Declaração dos vetores fora do laço para não ter lixo de memória dinâmica da heap		
		Vector3 dimHL = Vector3.zero, dimOC = Vector3.zero, dimVOL = Vector3.zero;
		Vector3 posiHL = Vector3.zero, posiOC = Vector3.zero, posiVOL = Vector3.zero;
		dimHL.z = dimOC.z = dimVOL.z = 1f;
		posiHL.z = posiOC.z = posiVOL.z = 0f;
		float iniYVolumes = telaExibicao.position.y - telaExibicao.lossyScale.y / 2f;
		float iniYCandles = iniYVolumes + uteis.espacoVolumes + ESPACO_ENTRE_VOLUMES_CANDLES;
		float posiX = telaExibicao.position.x - telaExibicao.lossyScale.x / 2f + uteis.espessura / 2f;
		SpriteRenderer sprite = quad.GetComponent<SpriteRenderer>();
		FracaoGrafico f = null;
		foreach(FracaoGrafico elem in lista) { //Limpando a lista
			Destroy(elem.hl);
			Destroy(elem.oc);
			Destroy(elem.vol);
		}
		lista.Clear();
		foreach(OHLC elem in arr) {
			hl = elem.high - elem.low;
			vol = elem.volume;
			if(elem.open > elem.close) {
				alta = false;
				oc = elem.open - elem.close;
				cor = corBaixa;
			} else {
				alta = true;
				oc = elem.close - elem.open;
				cor = corAlta;
			}
			dimHL.x = ESPESSURA_HL;
			dimHL.y = hl * uteis.alfaHL;

			dimOC.x = uteis.espessura;
			dimOC.y = oc * uteis.alfaHL;

			dimVOL.x = uteis.espessura;
			dimVOL.y = elem.volume * uteis.alfaVOL;
			
			posiHL.x = posiX;
			posiHL.y = iniYCandles + (elem.high - uteis.menorL) * uteis.alfaHL;
			
			posiHL.y -= dimHL.y / 2f;

			posiOC.x = posiX;
			if(alta) {
				posiOC.y = iniYCandles + (elem.close - uteis.menorL) * uteis.alfaHL;
				cor = corAlta;	
			} else {
				posiOC.y = iniYCandles + (elem.open - uteis.menorL) * uteis.alfaHL;
				cor = corBaixa;
			}
			posiOC.y -= dimOC.y / 2f;

			posiVOL.x = posiX;
			posiVOL.y = iniYVolumes + dimVOL.y / 2f;
			posiVOL.z = -2f;
			
			f = new FracaoGrafico();
			sprite.color = cor;
			f.hl = Instantiate(quad, posiHL, Quaternion.identity);
			f.oc = Instantiate(quad, posiOC, Quaternion.identity);
			sprite.color = corVolume;
			f.vol = Instantiate(quad, posiVOL, Quaternion.identity);
			f.hl.transform.localScale = dimHL;
			f.oc.transform.localScale = dimOC;
			f.vol.transform.localScale = dimVOL;
			f.oc.transform.SetParent(f.hl.transform);
			f.vol.transform.SetParent(f.hl.transform);
			f.hl.transform.SetParent(transform);
			lista.Add(f);
			posiX += uteis.espessura + uteis.ESPACO;
		}
		yield return null;
	}

}
