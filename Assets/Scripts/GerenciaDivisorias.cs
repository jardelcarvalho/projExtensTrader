using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GerenciaDivisorias : MonoBehaviour {

	[SerializeField] private RectTransform caixaDivLaterais;
	[SerializeField] private RectTransform caixaDivInferiores;
	[SerializeField] private GameObject divInfAPr;
	[SerializeField] private GameObject divInfBPr;
	[SerializeField] private GameObject divLatAPr;
	[SerializeField] private GameObject divLatBPr;
	[SerializeField] private GameObject vazioPrLat;
	[SerializeField] private GameObject vazioPrInf;

	private List<GameObject> listaLaterais = new List<GameObject>();
	private List<GameObject> listaInferiores = new List<GameObject>();

	private static CalculosUteis uteis;

	private Transform trLaterais, trInferiores;

	public static void setCalculosObj(CalculosUteis obj) {
		uteis = obj;
	}

	private Color corAtual;

	public void setCor(Color cor) {
		corAtual = cor;
	}

	void Awake() {
		Vector3[] corners = new Vector3[4];
		//setando transform global caixa div laterais
		caixaDivLaterais.GetWorldCorners(corners);
		trLaterais = new GameObject().transform;
		trLaterais.rotation = caixaDivLaterais.transform.rotation;
		trLaterais.localScale = new Vector3(Mathf.Abs(corners[3].x - corners[0].x), Mathf.Abs(corners[1].y - corners[0].y), caixaDivLaterais.transform.localScale.z);				   
		trLaterais.position = new Vector3(corners[3].x - trLaterais.localScale.x / 2f, 
											corners[1].y - trLaterais.localScale.y / 2f, caixaDivLaterais.transform.position.z);

		//setando transform global caixa div inferiores
		caixaDivInferiores.GetWorldCorners(corners);
		trInferiores = new GameObject().transform;
		trInferiores.rotation = caixaDivInferiores.transform.rotation;
		trInferiores.localScale = new Vector3(Mathf.Abs(corners[3].x - corners[0].x), Mathf.Abs(corners[1].y - corners[0].y), caixaDivInferiores.transform.localScale.z);				   
		trInferiores.position = new Vector3(corners[3].x - trInferiores.localScale.x / 2f, 
											corners[1].y - trInferiores.localScale.y / 2f, caixaDivInferiores.transform.position.z);
	}

	public IEnumerator setDivLaterais(int qtd) {
		if(qtd == 0) {
			yield return null;
		}
		//O número de divisórias intermediárias é igual ao quadrado de (qtd + 1)
		int divIntervalares = (qtd + 1) * (qtd + 1); 
		//O número total de divisórias é o número de divisórias intervalares mais a qtd
		int numDiv = qtd + divIntervalares;
		bool refatorar = false;
		if(numDiv == listaLaterais.Count) {
			refatorar = true;
		} else {
			for(int i = 0; i < listaLaterais.Count; i++) {
				Destroy(listaLaterais[i]);
			}
			listaLaterais.Clear();
		}
		float total = uteis.deltaHL;
		float deltaT = total / (float) numDiv;
		float fator = total / (trLaterais.localScale.y - divLatAPr.transform.localScale.y);
		float deltaY = trLaterais.localScale.y / (float) numDiv;
		
		bool texto = true;
		float posiY = trLaterais.position.y - trLaterais.localScale.y / 2f;
		float y;
		GameObject tmp;
		for(int i = 0; i < numDiv; i++) {
			if(!(i % qtd == 0)) {
				texto = false;
				if(!refatorar) {
					listaLaterais.Add(Instantiate(vazioPrLat) as GameObject);
					listaLaterais[i].GetComponentInChildren<UnityEngine.UI.Image>().color = corAtual;
				}
			} else {
				texto = true;
				if(!refatorar) {
					listaLaterais.Add(Instantiate(divLatBPr) as GameObject);
					listaLaterais[i].GetComponentInChildren<UnityEngine.UI.Image>().color = corAtual;
					listaLaterais[i].GetComponent<UnityEngine.UI.Text>().color = corAtual;
				}
			}
			tmp = listaLaterais[i];
			y = i * deltaY;
			if(!refatorar) {
				tmp.transform.SetParent(caixaDivLaterais.transform);
				if(!(i % qtd == 0)) {
					tmp.transform.localScale = new Vector3(0.99984f, 0.99984f, 0.99984f	);
				} else {	
					tmp.transform.localScale = new Vector3(0.6442989f, 0.6442989f, 0.6442989f);
				}

				tmp.transform.localPosition = new Vector3(tmp.transform.position.x,
																	0f,
																	tmp.transform.localPosition.z);

				tmp.transform.position = new Vector3(tmp.transform.position.x,
																posiY + y,
																tmp.transform.position.z);
			}

			if(texto) {
				float val = y * fator + uteis.menorL;
				int dot = val.ToString().IndexOf('.');
				if(dot + 1 >= 4) {
					tmp.GetComponent<UnityEngine.UI.Text>().text = (y * fator + uteis.menorL).ToString("N0");
				} else {
					string str = "N" + (4 - dot).ToString();
					tmp.GetComponent<UnityEngine.UI.Text>().text = (y * fator + uteis.menorL).ToString(str);
				}
				//tmp.GetComponent<UnityEngine.UI.Text>().text = (y * fator + uteis.menorL).ToString("N" + (4 - dot + 1).ToString());
			}
		}
		yield return null;
	}


	private int lastQtd = -1, lastElemTela = -1;
	private float lastDcr = 0f;
	private List<GameObject> outros = new List<GameObject>();
	public IEnumerator setDivInferiores(List<System.DateTime> datas, int elemNaTela, int qtd, int capacidadeMaxima, float decr) {
		bool x = lastDcr == decr;
		if(qtd == 0) {
			yield return null;
		}

		float avanco = uteis.espessura + uteis.ESPACO;
		float iniX = trInferiores.position.x - trInferiores.localScale.x / 2f;// - divInfAPr.transform.lossyScale.x / 2f + uteis.espessura / 2f;
		bool refatorar = false, texto;
		iniX += avanco / 2f;

		if(listaInferiores.Count == 0) {
			for(int i = 0; i < capacidadeMaxima; i++) {
				listaInferiores.Add(Instantiate(divInfAPr, caixaDivInferiores.transform) as GameObject);
				listaInferiores[i].SetActive(false);
			}
		}

		if(datas.Count == lastElemTela && lastQtd == qtd) {
			refatorar = true;
		} else {
			lastQtd = qtd;
			lastElemTela = elemNaTela;
		}

		for(int i = 0; i < outros.Count; i++) {
			Destroy(outros[i]);
		}
		outros.Clear();

		GameObject tmp;
		for(int i = 0; i < datas.Count; i++) {
			tmp = listaInferiores[i];
			tmp.SetActive(true);
			if(i % qtd == 0) {
				if(!refatorar) {
					tmp.GetComponent<UnityEngine.UI.Text>().color = corAtual;
					tmp.GetComponentInChildren<UnityEngine.UI.Image>().color = corAtual;
				}
				texto = true;
			} else {
				tmp.SetActive(false);
				if(!refatorar) {
					tmp.GetComponentInChildren<UnityEngine.UI.Image>().color = corAtual;
				}
				texto = false;
				if(i == datas.Count - 1) {
					texto = true;
					tmp.SetActive(true);
				}
			}
			
			if(!refatorar || !x) {
				lastDcr = decr;
				tmp.transform.position = new Vector3(iniX + i * avanco, tmp.transform.position.y, tmp.transform.position.z);
				if(tmp.transform.position.x > trInferiores.position.x + trInferiores.localScale.x / 2f - decr) {
					tmp.SetActive(false);
				}
			}
			
			if(texto) {
				tmp.GetComponent<UnityEngine.UI.Text>().text = datas[i].Day.ToString() + "/" + datas[i].Month.ToString();
			}

		}
		for(int i = datas.Count; i < listaInferiores.Count; i++) {
			listaInferiores[i].SetActive(false);
		}
		datas.Clear();
		yield return null;
	}

}
