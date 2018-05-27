using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfiguracaoInicialUI : MonoBehaviour {

	[SerializeField] private RectTransform caixaLateralBotao;
	[SerializeField] private Transform caixaGraficoDeVelas;
	[SerializeField] private RectTransform caixaExibicao;
	[SerializeField] private Transform caixaOcultamentoInferior;

	[SerializeField] private RectTransform caixaDivLaterais;

	[SerializeField] private RectTransform lateralAumentar;
	[SerializeField] private RectTransform lateralDiminuir;

	private void configuraBotoesDivisorias() {
		Vector3[] cantos = new Vector3[4];
		
		caixaLateralBotao.GetLocalCorners(cantos);
		Vector2 size = new Vector2(Mathf.Abs(cantos[3].x - cantos[0].x), Mathf.Abs(cantos[1].y - cantos[0].y));
		size.y /= 2f;
		lateralAumentar.sizeDelta = size;
		lateralDiminuir.sizeDelta = size;
		caixaLateralBotao.gameObject.GetComponent<GridLayoutGroup>().cellSize = size;
	}

	private void configuraCaixaGraficoDeVelas() {
		Vector3[] cantos = new Vector3[4];
		caixaExibicao.GetWorldCorners(cantos);
		Vector2 size = new Vector2(Mathf.Abs(cantos[3].x - cantos[0].x), Mathf.Abs(cantos[1].y - cantos[0].y));
		size -= new Vector2(0.2f, 0.2f); // offset
		caixaGraficoDeVelas.transform.localScale = new Vector3(size.x, size.y, 1f);
		caixaGraficoDeVelas.transform.position = new Vector3(cantos[0].x + size.x / 2f + 0.1f, cantos[0].y + size.y / 2f);
	}

	public void configuraCaixaDeOcultamento(float h) {
		Vector3 nPosi = caixaOcultamentoInferior.position;
		//nPosi.y += h;
		caixaOcultamentoInferior.position = nPosi;
	}

	public void efetuaConfiguracaoInicial() {
		//configuraBotoesDivisorias();
		configuraCaixaGraficoDeVelas();
	}
	
}
