using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AplicacacaoControlador : MonoBehaviour {

	public Dados dados;
	public CalculosUteis calculos;
	public int indiceAtual;
	public int elemNaTela;
	public const int MAX_ELEM_TELA = MAX_DIV_INF * MAX_DIV_INF;
	public const int MIN_ELEM_TELA = 1;
	public bool SMA1 = true, SMA2 = true, SMA3 = true;
	public int divisoriasLaterais = 0;
	public int divisoriasInferiores = 0;
	public const int MAX_DIV_INF = 12;
	public const int MIN_DIV_INF = 1;
	public const int MAX_DIV_LAT = 6;
	public const int MIN_DIV_LAT = 2;


	[SerializeField] private Transform telaExibicao;
	[SerializeField] private GraficoDeVelas graficoDeVelas;
	[SerializeField] private CurvasMedia curvasMedia;
	[SerializeField] private LinhaTendencia linhasTendencia;
	[SerializeField] private ConfiguracaoInicialUI configuracaoUI;
	[SerializeField] private GerenciaDivisorias divisoriasGer;
	[SerializeField] private UIController uiController;

	//Teste 1 = retornando quantidade de OHLC desejada (retornaVetorOHLC()), RETORNA NULL se não for possível
	//Teste 2 = calculo dos valores úteis consistente
	//Teste 3 = instanciamento do gráfico de velas com volume consistente (funcionando como corotina)
	//Teste 4 = traçagem das curvas de média consistente
	//Teste 5 = dinâmica das linhas de tendencia consistente

	public string dadosNomeAtivo() {
		return dados.getNomeAtivoAtual();
	}

	void Awake() {
		configuracaoUI.efetuaConfiguracaoInicial(); 
		dados = new Dados(); //Acesso a dados atuais
		calculos = new CalculosUteis(telaExibicao.lossyScale); //Acesso a valores importantes
		configuracaoUI.configuraCaixaDeOcultamento(calculos.espacoVolumes + GraficoDeVelas.ESPACO_ENTRE_VOLUMES_CANDLES);
		GraficoDeVelas.setTransformTelaExibicao(telaExibicao);
		CurvasMedia.setPosiXFinalInicial(telaExibicao.position.x + telaExibicao.lossyScale.x / 2f, telaExibicao.position.x - telaExibicao.lossyScale.x / 2f);
		CurvasMedia.setPosiYInicialCandles(telaExibicao.position.y - telaExibicao.lossyScale.y / 2f + calculos.espacoVolumes + GraficoDeVelas.ESPACO_ENTRE_VOLUMES_CANDLES);
		LinhaTendencia.setPosX(telaExibicao.position.x - telaExibicao.lossyScale.x / 2f, telaExibicao.position.x + telaExibicao.lossyScale.x / 2f);
		LinhaTendencia.setPosY(telaExibicao.position.y - telaExibicao.lossyScale.y / 2f + calculos.espacoVolumes + GraficoDeVelas.ESPACO_ENTRE_VOLUMES_CANDLES, telaExibicao.position.y + telaExibicao.lossyScale.y / 2f);
		LinhaTendencia.setCalculosObj(calculos);
		UIController.setCalculosObj(calculos);
		GerenciaDivisorias.setCalculosObj(calculos);
		indiceAtual = 0;
		elemNaTela = 1;
		divisoriasInferiores = MIN_DIV_INF;
		divisoriasLaterais = MIN_DIV_LAT;
		setarTema1();
		iteracoesIniciais = 0;
		indiceAtual = MAX_ELEM_TELA - 1;
	}

	float iteracoesIniciais;
	bool fimAnimacao = false;
	void Update() {
		if(!fimAnimacao) {
			if(iteracoesIniciais == 0) {
				iteracoesIniciais++;
				uiController.Avancar();
			} else if(iteracoesIniciais > 0 && iteracoesIniciais <= MAX_ELEM_TELA / 6) {
				iteracoesIniciais++;
				aumentarElemTela();
			} else if(iteracoesIniciais > MAX_ELEM_TELA / 2 && iteracoesIniciais <= MAX_ELEM_TELA / 2 + MAX_DIV_LAT) {
				iteracoesIniciais++;
				incrementaDivLaterais();
			} else if(iteracoesIniciais > MAX_ELEM_TELA / 2 + MAX_DIV_LAT && iteracoesIniciais <= MAX_ELEM_TELA / 2 + MAX_DIV_LAT + MAX_DIV_LAT / 2) {
				iteracoesIniciais++;
				decrementaDivLaterais();
			} else {
				fimAnimacao = true;
			}
		}
	}

	#region TEMAS
		private Color corSMA1_1 = Color.magenta;
		private Color corSMA2_1 = Color.grey;
		private Color corSMA3_1 = Color.yellow;
		private Color corDivisorias_1 = Color.white;
		private Color corCandleAlta_1 = Color.green;
		private Color corCandleBaixa_1 = Color.red;
		private Color corVolume_1 = Color.blue;
		private Color corLT_1 = Color.white;
		private Color corUIfundo_1 = Color.black;
		private Color corUIletrasPaineis_1 = Color.white;
		private Color corUIcampos_1 = Color.grey;
		private Color corUIletrasCampos_1 = Color.black;
		private Color corUIbotoes_1 = Color.white;

		private Color corSMA1_2 = Color.magenta;
		private Color corSMA2_2 = Color.grey;
		private Color corSMA3_2 = Color.yellow;
		private Color corDivisorias_2 = Color.black;
		private Color corCandleAlta_2 = Color.green;
		private Color corCandleBaixa_2 = Color.red;
		private Color corVolume_2 = Color.blue;
		private Color corLT_2 = Color.black;
		private Color corUIfundo_2 = Color.white;
		private Color corUIletrasPaineis_2 = Color.black;
		private Color corUIcampos_2 = Color.grey;
		private Color corUIletrasCampos_2 = Color.black;
		private Color corUIbotoes_2 = new Color(1f, 1f, 1f, 238f / 255f);
		
		private void setarTema1() {
			curvasMedia.setCor(corSMA1_1, corSMA2_1, corSMA3_1);
			uiController.legendaSetCor(corSMA1_1, corSMA2_1, corSMA3_1);
			divisoriasGer.setCor(corDivisorias_1);
			graficoDeVelas.setCor(corCandleAlta_1, corCandleBaixa_1, corVolume_1);
			linhasTendencia.setCor(corLT_1);
			uiController.setCor(corUIfundo_1, corUIletrasPaineis_1, corUIcampos_1, corUIletrasCampos_1, corUIbotoes_1);
		}

		private void setarTema2() {
			curvasMedia.setCor(corSMA1_2, corSMA2_2, corSMA3_2);
			divisoriasGer.setCor(corDivisorias_2);
			graficoDeVelas.setCor(corCandleAlta_2, corCandleBaixa_2, corVolume_2);
			linhasTendencia.setCor(corLT_2);
			uiController.setCor(corUIfundo_2, corUIletrasPaineis_2, corUIcampos_2, corUIletrasCampos_2, corUIbotoes_2);
		}
	#endregion

	private int temaAtual = 1;
	public void trocarTema() {
		if(temaAtual == 1) {
			temaAtual = 2;
			//configurar cores
			setarTema1();
		} else if(temaAtual == 2) {
			temaAtual = 1;
			//configurar cores
			setarTema2();
		}
	}

	public void setSMA1(bool ativado) {
		SMA1 = ativado;
		desenharCurvasMedia();
	}

	public void setSMA2(bool ativado) {
		SMA2 = ativado;
		desenharCurvasMedia();
	}

	public void setSMA3(bool ativado) {
		SMA3 = ativado;
		desenharCurvasMedia();
	}



	private const int VELOCIDADE = 8;
	public void aumentarElemTela() {
		for(int i = 0; i < VELOCIDADE; i++) {
			if(elemNaTela < MAX_ELEM_TELA && elemNaTela < indiceAtual + 1) {
				elemNaTela++;
			}
			if(elemNaTela % MAX_DIV_INF == 0 && divisoriasInferiores < MAX_DIV_INF) {
				divisoriasInferiores++;
			}
		}
		atualizaLinhasTendencia(1, false);	
		atualizarValoresUteis();
		desenharCandlesEVolumes();
		desenharCurvasMedia();
		StartCoroutine(divisoriasGer.setDivLaterais(divisoriasLaterais));
		StartCoroutine(divisoriasGer.setDivInferiores(buscaEmDatas(), elemNaTela, divisoriasInferiores, MAX_ELEM_TELA, decr));
	}

	public void diminuirElemTela() {
		for(int i = 0; i < VELOCIDADE; i++) {
			if(elemNaTela > MIN_ELEM_TELA) {
				elemNaTela--;
			}
			if(elemNaTela % MAX_DIV_INF == 0 && divisoriasInferiores < MAX_DIV_INF) {
				divisoriasInferiores--;
			}
		}
		atualizaLinhasTendencia(-1, false);
		atualizarValoresUteis();
		desenharCandlesEVolumes();
		desenharCurvasMedia();
		StartCoroutine(divisoriasGer.setDivLaterais(divisoriasLaterais));
		StartCoroutine(divisoriasGer.setDivInferiores(buscaEmDatas(), elemNaTela, divisoriasInferiores, MAX_ELEM_TELA, decr));
	}

	public void proximaEtapaGrafico() {
		avancarIndice();
		atualizarValoresUteis();
		desenharCandlesEVolumes();
		desenharCurvasMedia();
		atualizaLinhasTendencia(-1, true);
		StartCoroutine(divisoriasGer.setDivLaterais(divisoriasLaterais));
		StartCoroutine(divisoriasGer.setDivInferiores(buscaEmDatas(), elemNaTela, divisoriasInferiores, MAX_ELEM_TELA, decr));
	}

	private void avancarIndice() {
		indiceAtual++;
	}

	private void atualizarValoresUteis() {
		calculos.determinaUteis(dados.retornaVetorOHLCParaGraficoVelas(indiceAtual, elemNaTela), espacoC);
	}

	private void desenharCandlesEVolumes() {
		StartCoroutine(graficoDeVelas.desenharGraficoVelas(dados.retornaVetorOHLCParaGraficoVelas(indiceAtual, elemNaTela), calculos));
	}

	private void atualizaLinhasTendencia(int n, bool carregarRetasParaTras) {
		StartCoroutine(linhasTendencia.atualizaLinhas(n, carregarRetasParaTras));
	}

	public void incrementaDivLaterais() {
		if(divisoriasLaterais < MAX_DIV_LAT) {
			divisoriasLaterais++;
			StartCoroutine(divisoriasGer.setDivLaterais(divisoriasLaterais));
		}
	}

	public void decrementaDivLaterais() {
		if(divisoriasLaterais > MIN_DIV_LAT) {
			divisoriasLaterais--;
			StartCoroutine(divisoriasGer.setDivLaterais(divisoriasLaterais));
		}
	}

	private List<System.DateTime> buscaEmDatas() {
		List<OHLC> ls = new List<OHLC>();
		ls.AddRange(dados.retornaVetorOHLCParaCurvas(indiceAtual + 1 - elemNaTela, elemNaTela));
		List<System.DateTime> ret = new List<System.DateTime>();
		for(int i = 0; i < ls.Count; i++) {
			ret.Add(ls[i].time);
		}
		ls.Clear();
		return ret;
	}

	private void incrementaDivInferiores() {
		if(divisoriasInferiores < MAX_DIV_INF) {
			divisoriasInferiores++;
			StartCoroutine(divisoriasGer.setDivInferiores(buscaEmDatas(), elemNaTela, divisoriasInferiores, MAX_ELEM_TELA, decr));
		}
	}

	private void decrementaDivInferiores() {
		if(divisoriasInferiores > MIN_DIV_INF) {
			divisoriasInferiores--;
			StartCoroutine(divisoriasGer.setDivInferiores(buscaEmDatas(), elemNaTela, divisoriasInferiores, MAX_ELEM_TELA, decr));
		}
	}

	public void desenharCurvasMedia() {
		List<int> medias = new List<int>();
		List<bool> desenhar = new List<bool>();
		desenhar.Add(SMA1);
		desenhar.Add(SMA2);
		desenhar.Add(SMA3);
		int a = uiController.getNumSMA1();
		int b = uiController.getNumSMA2();
		int c = uiController.getNumSMA3();
		//if(a <= 0) desenhar[0] = false;
		//if(b <= 0) desenhar[1] = false;
		//if(c <= 0) desenhar[2] = false;
		medias.Add(a);
		medias.Add(b);
		medias.Add(c);
		StartCoroutine(curvasMedia.desenhaCurvasMedia(calculos, dados, medias.ToArray(), desenhar.ToArray(), elemNaTela, indiceAtual));
	}

	public void tracaLinhaTendencia(Vector3[] pontos, bool tracando) {
		linhasTendencia.desenha(pontos, tracando);
	}

	
	public void proximoGrafico() {
		//uiController.setLoading(true);
		dados.novaLeitura();
		iteracoesIniciais = 0;
		elemNaTela = MIN_ELEM_TELA;
		indiceAtual = MAX_ELEM_TELA - 1;
		fimAnimacao = false;
		linhasTendencia.deletarTodas();
		//uiController.setLoading(false);
	}

	public bool espacoC = false;
	private float decr = 0f;
	public void espacoCanto() {
		espacoC = !espacoC;
		decr = espacoC ? 1f : 0f;
		CurvasMedia.setPosiXFinalInicial(telaExibicao.position.x + telaExibicao.lossyScale.x / 2f - decr, telaExibicao.position.x - telaExibicao.lossyScale.x / 2f);
		atualizarValoresUteis();
		desenharCandlesEVolumes();
		desenharCurvasMedia();
		atualizaLinhasTendencia(-1, true);
		StartCoroutine(divisoriasGer.setDivLaterais(divisoriasLaterais));
		StartCoroutine(divisoriasGer.setDivInferiores(buscaEmDatas(), elemNaTela, divisoriasInferiores, MAX_ELEM_TELA, decr));
		StartCoroutine(graficoDeVelas.desenharGraficoVelas(dados.retornaVetorOHLCParaGraficoVelas(indiceAtual, elemNaTela), calculos));
	}

	/*void Update() {

		if(Input.GetKey(KeyCode.W)) {
			proximaEtapaGrafico();
		} else if(Input.GetKey(KeyCode.A)) {
			aumentarElemTela();
		} else if(Input.GetKey(KeyCode.D)) {
			diminuirElemTela();
		}

		if(Input.GetKey(KeyCode.I)) {
			incrementaDivLaterais();
		} else if(Input.GetKey(KeyCode.K)) {
			decrementaDivLaterais();
		} else if(Input.GetKey(KeyCode.J)) {
			decrementaDivInferiores();
		} else if(Input.GetKey(KeyCode.L)) {
			incrementaDivInferiores();
		}

		if(Input.GetKeyDown(KeyCode.X)) {	
			linhasTendencia.funcaoTeste();
		}

	}*/
	


}
