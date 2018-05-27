using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CurvasMedia : MonoBehaviour {

	private static float posiXFinal; //Setado em awake no script AplicacaoControlador
	private static float posiXInicial; //Setado em awake no script AplicacaoControlador
	private static float posiYInicial; //Setado em awake no script AplicacaoControlador
	private const float ESPESSURA_LINHA = 0.04f;
	private List<GameObject> curvas = new List<GameObject>();
	[SerializeField] private GameObject curvaPr;

	private Color[] cores;

	public void setCor(Color a, Color b, Color c) {
		cores = new Color[3] {a, b, c};
	}

	public static void setPosiXFinalInicial(float posiXF, float posiXI) {
		posiXFinal = posiXF;
		posiXInicial = posiXI;
	}

	public static void setPosiYInicialCandles(float posiY) {
		posiYInicial = posiY;
	}

	public IEnumerator desenhaCurvasMedia(CalculosUteis uteis, Dados dados, int[] quantidades, bool[] desenhar, int elemNaTela, int indiceAtual) {
		int quantidadesLength = quantidades.Length;
		if(quantidadesLength == 0) {
			yield return null;
		}
		Array.Sort(quantidades);
		OHLC[] elementos = null;
		int quantidadeBuscada = elemNaTela + quantidades[quantidadesLength - 1];
		int inicio;
		if(quantidadeBuscada > indiceAtual + 1) {
			inicio = 0;
			quantidadeBuscada = indiceAtual + 1;
		} else {
			inicio = indiceAtual - quantidadeBuscada;
		}
		//Buscando vetor com elementos para calculo das médias
		elementos = dados.retornaVetorOHLCParaCurvas(inicio, quantidadeBuscada);
		//Preparando variáveis para usar no calculo de média
		bool[] mediaIncremental = new bool[quantidadesLength];
		int[] N = new int[quantidadesLength];
		float[] soma = new float[quantidadesLength];
		for(int i = 0; i < quantidadesLength; i++) {
			N[i] = 0;
			soma[i] = 0f;
		}
		int indiceDecremento;
		float incrementoPosiX = uteis.ESPACO + uteis.espessura;
		float posiX;
		float maiorMedia = float.MinValue, menorMedia = float.MaxValue;
		limpaListaCurvas();
		List<Vector3> mediasPontos = new List<Vector3>();
		float mediaTmp, deltaMedia, alfaMedia;
		Vector3[] arrTmp;
		for(int i = 0; i < quantidadesLength; i++) {
			if(!desenhar[i]) {
				continue;
			}
			posiX = posiXFinal - (uteis.ESPACO + uteis.espessura) * elementos.Length + uteis.ESPACO + uteis.espessura / 2f;
			indiceDecremento = 0;
			maiorMedia = float.MinValue;
			menorMedia = float.MaxValue;
			foreach(OHLC elem in elementos) {
				soma[i] += elem.close;
				N[i]++;
				if(N[i] == quantidades[i]) {
					if(posiX >= posiXInicial) {
						mediaTmp = soma[i] / N[i];
						mediasPontos.Add(new Vector3(posiX, mediaTmp, 0f));
						if(mediaTmp > maiorMedia) {
							maiorMedia = mediaTmp;
						}
						if(mediaTmp < menorMedia) {
							menorMedia = mediaTmp;
						}
					}
					N[i]--;
					soma[i] -= elementos[indiceDecremento].close;
					indiceDecremento++;
				} else {
					if(posiX >= posiXInicial) {
						mediaTmp = soma[i] / N[i];
						mediasPontos.Add(new Vector3(posiX, mediaTmp, 0f));
						if(mediaTmp > maiorMedia) {
							maiorMedia = mediaTmp;
						}
						if(mediaTmp < menorMedia) {
							menorMedia = mediaTmp;
						}
					}
				}
				posiX += incrementoPosiX;
			}
			deltaMedia = maiorMedia - menorMedia;
			if(deltaMedia == 0f) {
				deltaMedia = 0.00000001f;
			}
			alfaMedia = uteis.espacoCandles / deltaMedia;

			arrTmp = mediasPontos.ToArray();
			for(int p = 0; p < arrTmp.Length; p++) {
				arrTmp[p] = new Vector3(arrTmp[p].x, posiYInicial + (arrTmp[p].y - menorMedia) * alfaMedia, 0f);
			}
			tracarLinhas(arrTmp, cores[i]);
			mediasPontos.Clear();
		}
		yield return null;
	}

	private void limpaListaCurvas() {
		foreach(GameObject curva in curvas) {
			Destroy(curva);
		}
		curvas.Clear();
	}

	private void tracarLinhas(Vector3[] pontos, Color cor) {
		if(pontos.Length == 0) {
			return;
		}
		GameObject curvaGo = Instantiate(curvaPr) as GameObject;
		LineRenderer curva = curvaGo.GetComponent<LineRenderer>();
		curva.startColor = cor;
		curva.endColor = cor;
		curva.positionCount = pontos.Length;
		curva.SetPositions(pontos);
		curvaGo.transform.SetParent(transform);
		curvas.Add(curvaGo);
	}

}
