# SolarAR — Especificação de Interface

> Fonte de verdade visual: referências exportadas do Figma em `docs/design/figma-styles.jpg`, `docs/design/figma-components.jpg` e `docs/design/figma-screens.jpg`.
>
> Este documento descreve a interface para futura implementação em Unity. Ele não define lógica de tracking nem substitui a validação dos valores no arquivo editável do Figma. Onde as exportações não permitem uma leitura segura, consta **A confirmar no Figma**.

## 1. Visão geral

A interface do SolarAR deve conduzir a criança desde a entrada no aplicativo até a exploração de um planeta por câmera ou galeria, com instruções curtas, ações explícitas e feedback contínuo sobre o estado do sistema.

O público-alvo inclui crianças neurodivergentes. A experiência deve, portanto, favorecer previsibilidade, clareza, consistência e baixo excesso de estímulos. Deve haver uma ação principal evidente por contexto, textos breves, poucos elementos simultâneos e controle do usuário sobre conteúdo e áudio. Esses princípios são diretrizes de produto, não afirmações clínicas, e devem ser validados com pessoas qualificadas e representantes do público-alvo quando possível.

Visualmente, o Figma combina um fundo noturno, superfícies claras, azul e ciano para navegação e feedback, amarelo para ações ou destaques e ilustrações de planetas. A proposta educacional aparece em instruções sequenciais, fatos curtos e interação direta com um planeta por vez.

## 2. Base de layout

### 2.1 Refinamentos aprovados para o MVP de sete planetas

Os refinamentos abaixo substituem as referências anteriores quando houver conflito:

- usar `Fredoka` em peso semibold ou bold para títulos e `Nunito` em peso regular ou semibold para corpo, ações e informações, evitando a aparência light/extra-light anterior;
- remover da permissão de câmera o título `Podemos usar a câmera?` e o bloco `Não tiramos nem guardamos fotos.`, sem adicionar título substituto; manter uma composição direta, com ilustração de câmera central, explicação curta e CTA `Abrir câmera`;
- reservar a lua-mascote para a experiência de curiosidades, sem usá-la para solicitar permissão;
- remover o CTA `Toque no planeta para saber mais` do estado reconhecido;
- usar exatamente `Assets/SolarAr/UI/Illustrations/CuriosityMoon.png` como lua-mascote, sem redesenho; mostrá-la no canto do estado reconhecido, acompanhada do indicador `Curiosidades`; ao tocar nela, substituir o convite por um painel editorial de aproximadamente meia tela com uma versão compacta do mesmo asset e duas curiosidades do `PlanetFacts` atual, reduzindo temporariamente o planeta para equilibrar a composição sem sobreposições;
- aplicar ao cartão de curiosidades um acento de cor associado ao planeta: cinza quente para Mercúrio, azul/verde para Terra, vermelho para Marte, marrom para Júpiter, dourado suave para Saturno, ciano para Urano e azul profundo para Netuno;
- permitir rotação manual em dois eixos por arraste, mantendo a rotação automática pausada durante a interação;
- exibir o planeta em AR com diâmetro visual inicial de aproximadamente `0,058 m`, centro visual a `0,035 m` acima do card e oscilação vertical suave de até `0,003 m`;
- nas visualizações sem câmera, desativar a oscilação e o offset AR e normalizar o diâmetro visual para aproximadamente `0,048 m`, mantendo o modelo centralizado no preview;
- priorizar fatos curtos, surpreendentes e adequados a crianças, evitando repetir apenas as características básicas já apresentadas na descrição do planeta;
- manter somente um planeta ativo, mas permitir a troca sequencial de cards na mesma sessão de câmera, atualizando modelo, estado e curiosidades sem retornar ao menu.

| Propriedade | Especificação |
|---|---|
| Plataforma de referência | Android |
| Base de layout | `412 × 915 px` |
| Grid | `8 px` |
| Safe Area superior | `32 px` para a área de status |
| Safe Area inferior | `24 px` para a área de gestos |
| Área mínima de toque | `48 × 48 px` |
| Altura mínima de botão | `56 px` |
| Margens laterais | **A confirmar no Figma** |
| Largura útil e colunas | **A confirmar no Figma** |

A base de `412 × 915 px` é explicitamente indicada na prancha de estilos. Na implementação, a Safe Area deve ser lida em tempo de execução. O conteúdo deve se adaptar a outras proporções Android sem distorção, sem corte de texto ou sobreposição com barras do sistema. Elementos devem preservar hierarquia, alinhamento e áreas mínimas de toque; a estratégia exata de ancoragem, expansão e quebra de texto é **A confirmar no Figma**.

O espaçamento deve usar apenas os passos documentados na seção de tokens. Não é possível associar com segurança cada passo a margens ou lacunas específicas usando somente as imagens exportadas.

### 2.2 Extensão aprovada — Quiz Galáctico MVP 1.0

O Quiz Galáctico amplia a experiência principal sem câmera e reutiliza integralmente os tokens e componentes existentes. A navegação inferior passa a ter três abas de mesma hierarquia: `Explorar`, `Planetas` e `Missões`. A aba `Missões` não reinicia a sessão AR e não mantém modelos AR ou previews da galeria visíveis sobre o quiz.

- usar `Assets/SolarAr/UI/Illustrations/CuriosityMoon.png` como mascote nas telas inicial e de resultado, sem redesenho;
- manter uma única ação principal evidente por estado: começar, responder, avançar ou jogar novamente;
- apresentar feedback e explicação educativa depois de qualquer resposta, sem avanço automático;
- bloquear as alternativas após a primeira escolha e manter `Próxima pergunta` desabilitado antes da resposta;
- representar o resultado com uma a três estrelas nas cores existentes, sem criar um novo estilo visual;
- descartar partidas incompletas ao sair da aba, sem contabilizá-las como concluídas.

### 2.3 Extensão aprovada — Central de Jogos Educativos MVP 1.0

A aba `Missões` passa a abrir a `Central de Missões Espaciais`, sem câmera, antes de iniciar qualquer atividade. A Central organiza quatro atividades em uma grade de duas colunas: `Quiz Galáctico`, `Memória Cósmica`, `Ordem Planetária` e `Detetive Espacial`.

- reutilizar os tokens, fontes, botões, cards e imagens de planetas existentes;
- manter a mascote oficial em `Assets/SolarAr/UI/Illustrations/CuriosityMoon.png` onde ela for exibida;
- usar cards inteiramente clicáveis, com nome e descrição textual além do símbolo visual;
- apresentar instruções curtas, progresso, feedback textual, resultado com estrelas, `Jogar novamente` e retorno à Central;
- iniciar cada entrada em um jogo com uma nova partida e descartar o estado incompleto ao sair;
- manter câmera, tracking e preview 3D ocultos durante toda a navegação da Central e dos jogos;
- informar na `Ordem Planetária` que a sequência usa apenas os sete planetas disponíveis no SolarAR e não representa a lista completa do Sistema Solar.

### 2.4 Extensão aprovada — Planetas 3D e carrossel educativo

As telas digitais passam a representar os planetas com os sete prefabs 3D já usados pelo SolarAR. As imagens dos cards físicos permanecem exclusivas do reconhecimento por imagem e não devem aparecer como ilustração da galeria ou dos jogos.

- gerar uma miniatura estática a partir de cada prefab 3D e reutilizar o mesmo cache na galeria, na Memória Cósmica, na Ordem Planetária e na revelação do Detetive Espacial;
- limitar a geração a uma câmera de preview e um modelo temporário por vez, evitando câmeras ou instâncias permanentes por card;
- manter somente um modelo 3D vivo na tela de detalhe, com rotação suave e interação por arraste;
- apresentar oito curiosidades por planeta em um carrossel previsível, com um cartão por vez, contador, categoria, botões anterior/próximo e deslize horizontal;
- preservar o planeta selecionado enquanto as curiosidades são percorridas e manter controles separados para trocar de planeta;
- usar fundo azul-marinho/roxo, acentos ciano, lilás e magenta, cartões arredondados e estrelas discretas, sem comprometer contraste ou aumentar a quantidade de estímulos simultâneos;
- manter Fredoka, Nunito e a lua-mascote oficial, sem copiar marcas ou elementos identificadores de referências externas.

## 3. Design tokens

### 3.1 Cores

| Token | Hex | Uso e exemplos |
|---|---|---|
| `background/night` | `#0A1433` | Fundo principal das telas, área de AR sem câmera e base da navegação. |
| `background/deep-space` | `#080B24` | Base inferior do gradiente espacial nas telas sem câmera. |
| `surface/space-card` | `#182852` | Cartões escuros da galeria, carrossel e Central de Missões. |
| `accent/purple` | `#6D4AFF` | Profundidade no gradiente e brilho discreto de superfícies espaciais. |
| `accent/magenta` | `#E650B7` | Acento alternado em ícones da Central de Missões. |
| `text/ink` | `#19365B` | Texto escuro sobre superfícies claras e títulos de cards. |
| `blue/1` | `#387ED8` | Controles azuis, ícones e elementos ativos. |
| `blue/4` | `#294671` | Superfícies azuis, cards da galeria e containers de apoio. |
| `text/on-dark` | `#FFFFFF` | Texto e ícones sobre fundo escuro. |
| `surface/panel` | `#F7FAFF` | Painéis, modais e cards claros. |
| `action/primary` | `#FFDB69` | Ação ou destaque primário amarelo. |
| `accent/cyan` | `#69D9EF` | Moldura de leitura, ícones e destaques ciano. |
| `action/secondary` | `#97E4F4` | Botões secundários e superfícies de ação em ciano claro. |
| `text/soft-lavender` | `#D4D1F3` | Texto secundário suave sobre fundo escuro. |
| `surface/fact-sun` | `#FFF4CD` | Card de fato educacional amarelo-claro. |
| `feedback/success` | `#B6E9C9` | Confirmação de card reconhecido. |

Não foram definidos nas referências tokens separados para erro, aviso, overlay, sombra ou estados pressionado/desabilitado; **A confirmar no Figma**.

### 3.2 Tipografia

| Uso | Família | Tamanho | Peso | Line height |
|---|---|---:|---|---|
| Títulos | Fredoka | `28 px`, `32 px` ou `36 px` | **A confirmar no Figma** | **A confirmar no Figma** |
| Corpo | Nunito | `18 px` | **A confirmar no Figma** | **A confirmar no Figma** |
| Ações e botões | Nunito | `20 px` | **A confirmar no Figma** | **A confirmar no Figma** |
| Status nativo Android | Nunito | `14 px` | **A confirmar no Figma** | **A confirmar no Figma** |
| Marca “SolarAR” | **A confirmar no Figma** | Tamanho próprio, não especificado | **A confirmar no Figma** | **A confirmar no Figma** |

Fredoka é a família de títulos e Nunito é a família de corpo, ações e status. A atribuição exata de `28`, `32` e `36 px` a cada nível de título é **A confirmar no Figma**. Labels, fatos educacionais e textos auxiliares aparentam seguir a hierarquia de Nunito, mas seus tamanhos exatos também são **A confirmar no Figma**.

### 3.3 Border radius e sombras

Os raios definidos são `20 px`, `24 px` e `28 px`. Eles aparecem em botões, cards, painéis, modais e containers, porém a correspondência exata entre cada componente e cada valor é **A confirmar no Figma**.

As sombras são descritas como suaves e nativas. Cor, deslocamento, blur e opacidade são **A confirmar no Figma**.

### 3.4 Espaçamento

Escala identificada: `0`, `8`, `16`, `24`, `32`, `40`, `48`, `56`, `64` e `80 px`.

Todos os afastamentos futuros devem preferir essa escala. A aplicação exata de cada valor em padding, gap e margem é **A confirmar no Figma**.

## 4. Componentes reutilizáveis

| Componente | Finalidade e aparência | Estados | Dimensões | Ocorrências |
|---|---|---|---|---|
| Botão de ação | Botão arredondado com label e ícone opcional; variantes amarela, ciano e clara. | Padrão, com ícone, sem ícone e desabilitado. Estado pressionado: **A confirmar no Figma**. | Altura mínima `56 px`; largura e raio: **A confirmar no Figma**. | Entrada, permissão, instruções, AR e painel educacional. |
| Botão de ícone | Controle circular ou arredondado para voltar, ajuda, fechar e câmera. | Padrão/ativo; demais estados: **A confirmar no Figma**. | Área de toque mínima `48 × 48 px`; visual interno: **A confirmar no Figma**. | Headers, painel educacional e estados AR. |
| Header | Linha superior com voltar, título central/contextual e ajuda quando aplicável. | Varia conforme tela. | Altura e paddings: **A confirmar no Figma**. | Instruções, Explorar, Planetas e detalhe. |
| Bottom navigation | Barra com três abas e labels: `Explorar`, `Planetas` e `Missões`; a aba ativa recebe fundo azul/ciano destacado. | Explorar ativa; Planetas ativa; Missões ativa. | Altura, largura e raio: **A confirmar no Figma**. | Fluxos principais após a entrada. |
| Moldura de leitura AR | Quatro cantos ciano delimitam a região onde o card deve ser posicionado. | Vazia, procurando e card perdido. | **A confirmar no Figma**. | Explorar e estados de busca/perda. |
| Mensagem de estado AR | Ícone, título curto, texto de apoio e, quando necessário, ação. | Inicial, procurando, reconhecido, perdido, pouca luz e troca de card. | **A confirmar no Figma**. | Experiência de câmera. |
| Confirmação de reconhecimento | Chip/faixa verde com check e texto `Card reconhecido!`. | Visível após reconhecimento; duração: **A confirmar no Figma**. | **A confirmar no Figma**. | Card reconhecido. |
| Card de planeta da galeria | Superfície azul com imagem do planeta e nome. | Padrão e selecionado/pressionado: **A confirmar no Figma**. | Grade `2 × 4`; dimensão individual: **A confirmar no Figma**. | Planetas. |
| Painel educacional | Superfície clara com miniatura, nome, fechar, dois fatos, áudio e dica de gestos. | Áudio disponível e áudio desabilitado; aberto/fechado. | **A confirmar no Figma**. | Informações em AR e detalhe sem câmera. |
| Card de fato | Bloco pastel com ícone e texto curto; variantes azul-claro e amarelo-claro. | Padrão. | **A confirmar no Figma**. | Painel educacional. |
| Controle de áudio | Botão `Ouvir (opcional)` com ícone de alto-falante; versão desabilitada tem aparência esmaecida. | Disponível e desabilitado. Reprodução/pausa: **A confirmar no Figma**. | Altura mínima `56 px`; demais medidas: **A confirmar no Figma**. | Entrada, instruções, estados AR e conteúdo educacional. |
| Card/modal instrucional | Superfície clara arredondada com título, etapas ou mensagem e ação. | Permissão, Como usar, Pouca luz e Troca de card. | **A confirmar no Figma**. | Onboarding e feedback AR. |
| Indicador de planeta/órbita | Imagem do planeta acompanhada por elipse de ancoragem/profundidade. | Planeta ativo. | **A confirmar no Figma**. | Informações e detalhe. |
| Navegação Anterior/Próximo | Dois botões para percorrer planetas, mantendo conteúdo sincronizado. | Primeiro, intermediário e último planeta: **A confirmar no Figma**. | **A confirmar no Figma**. | Detalhe de planeta. |

Os ícones representam câmera, ajuda, planeta/órbita, áudio, busca, luz, troca, fatos, voltar e fechar. Biblioteca, espessura, tamanhos e regras de exportação dos ícones são **A confirmar no Figma**.

## 5. Navegação

O fluxo de entrada oferece três caminhos: iniciar a câmera, ouvir conteúdo opcional e abrir `Como usar`. Ao entrar na experiência principal, a navegação inferior alterna entre:

- `Explorar`: experiência AR e seus estados de tracking;
- `Planetas`: galeria com os sete planetas e acesso ao detalhe sem câmera;
- `Missões`: Quiz Galáctico independente da câmera.

Fluxos identificados:

`Entrada → Permissão de câmera → Explorar → Procurando card → Card reconhecido → Informações do planeta`

`Entrada → Como usar → Explorar`

`Explorar ↔ Planetas → Detalhe do planeta → Anterior/Próximo`

`Explorar ↔ Planetas ↔ Missões → Central de Missões → Jogo → Resultado → Jogar novamente ou Central`

`Card perdido → Retorno à busca → Card recuperado`

`Planeta ativo → Troca de card → Retorno à busca → Novo planeta reconhecido`

O botão voltar aparece nos headers das telas internas. O controle fechar do painel educacional retorna à visualização do planeta. Destino exato do botão voltar em cada tela, comportamento do botão Android Back e persistência da aba ao retornar são **A confirmar no Figma**.

## 6. Telas e estados visuais

As referências originais declaram `12 telas nativas`. Os jogos educativos acrescentam a Central e os estados internos documentados em 6.13 a 6.18.

### 6.1 Entrada / SolarAR

- **Objetivo:** apresentar o aplicativo e iniciar a exploração.
- **Conteúdo:** saudação `Olá, pequeno explorador!`, ilustração, marca `SolarAR`, frase `O universo nas suas mãos!` e texto introdutório.
- **Ações:** `Explorar com a câmera`, `Ouvir (opcional)` e `Como usar`.
- **Comportamento:** a exploração por câmera conduz à solicitação de permissão quando necessária; nenhuma gravação de imagem deve ocorrer.

### 6.2 Permissão de câmera — “Vamos explorar?”

- **Objetivo:** explicar por que a câmera é necessária antes da permissão Android.
- **Conteúdo:** painel `Podemos usar a câmera?`, explicação sobre reconhecer cards e aviso `Não tiramos nem guardamos fotos.`
- **Ações:** `Permitir câmera`, `Ouvir (opcional)` e voltar.
- **Comportamento:** solicitar a permissão nativa apenas ao entrar no fluxo de câmera. Fluxo visual para permissão negada: **A confirmar no Figma**.

### 6.3 Como usar

- **Objetivo:** explicar a experiência em três passos.
- **Conteúdo:** `Seu planeta em 3 passos`: `Escolha um card`, `Aponte a câmera` e `Explore o planeta`; aviso para não cobrir o card com a mão.
- **Ações:** `Ouvir (opcional)` e voltar.
- **Comportamento:** conteúdo estático e curto; destino após voltar: **A confirmar no Figma**.

### 6.4 Explorar — câmera disponível

- **Objetivo:** introduzir a região de leitura antes do reconhecimento.
- **Conteúdo:** header `Explorar`, chamada `Vamos encontrar um planeta?`, instrução para pegar um card e preparar a câmera, moldura e placeholder `O planeta vai aparecer aqui!`.
- **Ações:** voltar, ajuda, `Ouvir (opcional)` e abas inferiores.
- **Estado do sistema:** câmera disponível, sem reconhecimento.

### 6.5 Procurando card

- **Objetivo:** orientar o enquadramento enquanto nenhum card válido foi reconhecido.
- **Conteúdo:** moldura, `Aponte a câmera para um card`, indicador `Procurando um card...` e instrução para manter o card inteiro na moldura e o celular parado.
- **Ações:** voltar, ajuda e abas inferiores.
- **Estado do sistema:** tracking ativo, aguardando card válido.

### 6.6 Card reconhecido

- **Objetivo:** confirmar o reconhecimento e mostrar o planeta ancorado ao card.
- **Conteúdo:** feed real da câmera, card físico, modelo do planeta e confirmação `Card reconhecido!`.
- **Ações:** tocar no planeta para saber mais, voltar, ajuda e abas inferiores.
- **Estado do sistema:** imagem rastreada e planeta correspondente ativo.

### 6.7 Informações do planeta em AR

- **Objetivo:** apresentar dados curtos do planeta ativo e permitir interação.
- **Conteúdo:** planeta, referência de órbita/profundidade e painel educacional de `Terra` com dois fatos, áudio e dica `Gire • Aproxime • Reduza`.
- **Ações:** fechar painel, ouvir, girar por arraste, redimensionar por pinça e alternar abas.
- **Estado do sistema:** planeta ativo; os dados devem permanecer sincronizados ao modelo reconhecido.

### 6.8 Card perdido

- **Objetivo:** informar a perda temporária de tracking sem gerar confusão.
- **Conteúdo:** `Aponte a câmera para um card`, `Mostre ele de novo para a câmera.` e `Não se preocupe! Seu planeta volta assim que encontrar o card.`
- **Ações:** reenquadrar o mesmo card, voltar, ajuda e alternar abas.
- **Estado do sistema:** tracking perdido; o modelo fica oculto até a recuperação.

### 6.9 Pouca luz

- **Objetivo:** orientar a correção das condições de iluminação.
- **Conteúdo:** `Está um pouquinho escuro!`, explicação de que é necessária mais luz e dicas para acender a luz ou ir para perto de uma janela, evitar sombras e tentar novamente.
- **Ações:** `Ouvir (opcional)`, melhorar a iluminação, voltar, ajuda e alternar abas.
- **Estado do sistema:** iluminação insuficiente. Critério técnico de detecção: **A confirmar no Figma**.

### 6.10 Troca de card

- **Objetivo:** garantir uma troca previsível entre planetas.
- **Conteúdo:** modal `Troca de card`, orientação para apontar outro card e mostrar somente um card por vez.
- **Ações:** remover o card anterior, apresentar o novo card, `Ouvir (opcional)`, voltar, ajuda e alternar abas.
- **Estado do sistema:** transição entre cards; o planeta anterior deve ser removido antes de mostrar o novo.

### 6.11 Planetas

- **Objetivo:** permitir conhecer os planetas sem usar a câmera.
- **Conteúdo:** título `Planetas`, instrução `Toque para ver de perto` e grade com Mercúrio, Terra, Marte, Júpiter, Saturno, Urano e Netuno.
- **Ações:** selecionar um planeta, voltar e alternar para `Explorar`.
- **Estado do sistema:** galeria estática; seleção abre apenas um planeta.

### 6.12 Detalhe do planeta

- **Objetivo:** apresentar um planeta e seu conteúdo sem câmera.
- **Conteúdo:** nome (`Terra` na referência), label `Visualização sem câmera`, planeta, painel educacional, áudio e dicas de gesto.
- **Ações:** girar, aproximar/reduzir, ouvir, fechar conteúdo, `Anterior`, `Próximo`, voltar e alternar abas.
- **Estado do sistema:** somente um planeta selecionado, sem tracking AR.

### 6.13 Central de Missões Espaciais

- **Objetivo:** permitir a escolha previsível entre as quatro atividades educativas.
- **Conteúdo:** título, introdução curta e grade `2 × 2` com símbolo, nome e descrição de cada jogo.
- **Ações:** abrir `Quiz Galáctico`, `Memória Cósmica`, `Ordem Planetária` ou `Detetive Espacial`; alternar abas inferiores.
- **Estado do sistema:** nenhuma partida ativa; câmera e preview 3D ocultos.

### 6.14 Quiz Galáctico — início, pergunta e feedback

- **Objetivo:** responder uma pergunta por vez com retorno educativo.
- **Conteúdo:** introdução com a lua-mascote oficial e resumo local; durante a partida, progresso textual e visual, enunciado, três alternativas, feedback da escolha e explicação científica curta.
- **Ações:** começar, selecionar uma única alternativa, avançar depois do feedback ou voltar à Central.
- **Estado do sistema:** alternativas bloqueadas após a resposta; nenhuma pontuação duplicada; saída da aba descarta a partida incompleta.

### 6.15 Quiz Galáctico — resultado

- **Objetivo:** comunicar o desempenho de maneira acolhedora e permitir uma nova partida.
- **Conteúdo:** título `Missão concluída!`, pontuação, quantidade de acertos, uma a três estrelas e mensagem da lua-mascote.
- **Ações:** `Jogar novamente` ou alternar entre as três abas.
- **Estado do sistema:** melhor pontuação e quantidade de partidas concluídas persistidas localmente, sem dados pessoais.

### 6.16 Memória Cósmica

- **Objetivo:** encontrar seis pares entre doze cartas usando seis planetas escolhidos por partida.
- **Conteúdo:** grade de cartas, pares encontrados, tentativas e instrução curta.
- **Ações:** revelar cartas, jogar novamente e voltar à Central.
- **Estado do sistema:** no máximo duas cartas em avaliação; interação bloqueada durante a ocultação de um par incorreto.

### 6.17 Ordem Planetária

- **Objetivo:** ordenar os sete planetas disponíveis por distância ao Sol.
- **Conteúdo:** aviso explícito sobre a ausência de Vênus no conjunto do SolarAR, sete posições, contador e feedback de conferência.
- **Ações:** selecionar origem e destino, conferir, tentar novamente, jogar novamente e voltar à Central.
- **Estado do sistema:** posições corretas e incorretas recebem feedback visual e textual; a partida termina somente com todas corretas.

### 6.18 Detetive Espacial

- **Objetivo:** identificar cinco planetas diferentes a partir de pistas progressivas.
- **Conteúdo:** progresso, pontuação, uma a três pistas, três alternativas e explicação após a resposta.
- **Ações:** pedir pista, responder uma vez, avançar manualmente, jogar novamente e voltar à Central.
- **Estado do sistema:** resposta e novas pistas bloqueadas depois da escolha; pontuação depende da quantidade de pistas reveladas.

## 7. Estados de AR e tracking

| Estado | Condição de entrada | Mensagem/feedback | Ação do usuário | Comportamento e saída esperada |
|---|---|---|---|---|
| `02_Explorar` | Câmera disponível e nenhum reconhecimento iniciado. | `Vamos encontrar um planeta?` e placeholder dentro da moldura. | Preparar um card e apontar a câmera. | Avança para procura quando a leitura está ativa. |
| `02a_Procurando` | Tracking ativo sem card válido. | `Aponte a câmera para um card` e `Procurando um card...`. | Manter o card inteiro na moldura e o celular parado. | Sai ao reconhecer card válido ou ao detectar condição de pouca luz. |
| `02b_Reconhecido` | Imagem de referência válida detectada. | Chip `Card reconhecido!`, planeta sobre o card e convite para tocar. | Tocar no planeta ou manipulá-lo. | Ancora o prefab correspondente; abre informações ao toque. |
| `02c_Informacoes` | Usuário toca no planeta ativo. | Painel com nome, fatos, áudio e gestos. | Ler, ouvir, girar, redimensionar ou fechar. | Fecha para o planeta ativo; perda de tracking conduz ao estado de card perdido. |
| `02d_Card_Perdido` | Tracking do card ativo deixa de ser confiável. | `Mostre ele de novo para a câmera` e mensagem tranquilizadora. | Reenquadrar o mesmo card. | Oculta o modelo até recuperar tracking; retorna ao planeta ativo quando recuperado. Timeout para voltar à busca: **A confirmar no Figma**. |
| `02e_Pouca_Luz` | Sistema identifica iluminação insuficiente. | `Está um pouquinho escuro!` e dicas textuais. | Melhorar a iluminação e tentar novamente. | Retorna à procura quando a condição permite leitura. Limiar e detecção: **A confirmar no Figma**. |
| `02f_Troca_De_Card` | Usuário apresenta outro card enquanto há um planeta ativo ou inicia troca. | Modal `Troca de card` e orientação de um card por vez. | Remover o card anterior e apresentar o novo. | Remove o planeta anterior antes de reconhecer e exibir o novo; retorna à procura durante a transição. |

Em todos os estados, somente o planeta associado ao identificador estável da imagem reconhecida pode aparecer. Não devem permanecer modelos incorretos ou duplicados após perda de tracking, troca ou remoção da imagem.

## 8. Conteúdo educacional

O conteúdo é apresentado em um painel claro sobre o contexto AR ou na visualização sem câmera. A referência da Terra contém:

- miniatura e nome do planeta;
- fato em card azul-claro: `É o único planeta conhecido que tem vida.`;
- fato em card amarelo-claro: `Tem 1 lua, chamada Lua.`;
- ação `Ouvir (opcional)`;
- dica de interação `Gire • Aproxime • Reduza`.

Cada detalhe mostra somente um planeta. Na galeria, `Anterior` e `Próximo` sincronizam modelo, título, fatos e áudio. Quantidade de fatos por planeta além do exemplo, textos dos demais planetas, duração dos conteúdos, disponibilidade das narrações e ordem de navegação circular ou limitada são **A confirmar no Figma**.

Mapeamento conceitual esperado:

- `PlanetContent`: nome, descrição e conteúdo apresentável de um planeta;
- `PlanetFacts`: fatos curtos e estruturados usados nos cards;
- `EducationalContentManager`: seleciona e fornece o conteúdo correspondente ao planeta ativo, sem embutir os dados no componente visual.

## 9. Acessibilidade

- Manter contraste legível entre texto e superfície e validar numericamente os pares antes da implementação; o Figma define cores, mas não informa razões de contraste.
- Respeitar área mínima de toque de `48 × 48 px` e altura mínima de botão de `56 px`.
- Usar textos curtos, diretos e consistentes, com uma ação principal evidente por contexto.
- Mostrar um planeta e um painel principal por vez, reduzindo competição visual.
- Não depender apenas da cor: reconhecimento, pouca luz e perda de tracking têm ícone e mensagem textual.
- Manter posição, aparência e significado consistentes para voltar, ajuda, fechar, áudio e navegação inferior.
- Tratar áudio como opcional, indicar quando estiver indisponível e permitir que o usuário controle seu acionamento.
- Evitar flashes, movimento intenso e animação automática sem finalidade. Duração e curvas de animação são **A confirmar no Figma**.
- Manter feedback explícito durante espera, reconhecimento, erro recuperável e transição de card.
- Garantir que aumento de texto e diferentes proporções não cortem informações; limites e comportamento exatos são **A confirmar no Figma**.
- Validar as decisões com pessoas qualificadas e representantes do público-alvo quando possível.

## 10. Mapeamento para a arquitetura do SolarAR

| Elemento arquitetural | Responsabilidade esperada na interface |
|---|---|
| `MainMenu` | Apresentar a entrada e iniciar os fluxos de câmera, áudio opcional e instruções. |
| `Instructions` | Exibir permissão contextual e os três passos de uso, sem implementar a permissão ou o tracking. |
| `PlanetHUD` | Mostrar header, controles, planeta ativo, painel educacional e dicas de gesto. |
| `ARFeedback` | Traduzir o estado recebido da camada AR em mensagens de procura, reconhecimento, perda, pouca luz e troca. Não decide tracking. |
| `EducationalContentManager` | Selecionar o conteúdo do planeta ativo e entregá-lo à UI. A UI apenas apresenta esse conteúdo. |
| `PlanetContent` | Armazenar o conteúdo educacional associado ao planeta. |
| `PlanetFacts` | Estruturar fatos curtos exibidos nos cards educacionais. |
| `SolarARManager` / `AppState` | Coordenar a navegação e o estado geral quando necessário, mantendo responsabilidades explícitas. |
| `ARImageTrackingManager` / `TrackedImageHandler` | Informar detecção, atualização e perda de imagens à UI por uma fronteira clara; não conter textos educacionais. |
| `GestureController` e controladores de interação | Interpretar toque, arraste e pinça; a UI apenas comunica as affordances e o estado. |
| `QuizQuestionBank` / `QuizQuestionSelector` | Armazenar e selecionar perguntas sem depender da camada visual. |
| `QuizManager` / `QuizSession` | Coordenar a partida, bloquear respostas repetidas, calcular progresso e registrar somente partidas concluídas. |
| `QuizScreenController` | Apresentar os estados inicial, pergunta, feedback e resultado; não contém o banco de perguntas. |
| `MissionHubController` | Coordenar Central, entrada e saída dos quatro jogos; descartar a partida incompleta ao trocar de jogo ou aba. |
| Sessões de Memória, Ordem e Detetive | Manter regras, estado, pontuação e conclusão sem depender da apresentação visual. |
| Controladores de tela dos jogos | Apresentar o estado recebido das sessões e encaminhar as interações; não duplicar regras. |

Este mapeamento registra responsabilidades, não exige criação imediata de classes nem define APIs.

## 11. Regras para futura implementação em Unity

1. Consultar este documento e as referências exportadas antes de implementar ou revisar qualquer UI.
2. Tratar o Figma como fonte principal de verdade visual; atualizar esta especificação quando houver decisão aprovada no design.
3. Não substituir valores marcados como **A confirmar no Figma** por estimativas silenciosas.
4. Centralizar tokens repetidos de cor, tipografia, raio e espaçamento quando isso reduzir divergências sem criar arquitetura excessiva.
5. Reutilizar botões, cards, headers, navegação, mensagens e painéis em vez de reproduzi-los por tela.
6. Ler e aplicar a Safe Area em tempo de execução.
7. Preservar no mínimo `48 × 48 px` para toque e `56 px` de altura para botões.
8. Manter cores, tipografia, ícones e hierarquia consistentes; não introduzir padrões visuais novos sem necessidade aprovada.
9. Usar `412 × 915 px` como base, com layout responsivo para diferentes proporções Android e sem deformar assets.
10. Separar apresentação e navegação da lógica de tracking. A UI reage ao estado AR, mas não reconhece cards.
11. Não colocar seleção ou dados educacionais diretamente em componentes visuais; obtê-los da camada Education.
12. Exibir somente o planeta associado ao card atual e limpar corretamente o anterior em perda ou troca.
13. Usar o feed real da câmera na camada de ambiente. Os globos das pranchas são pré-visualizações e devem ser substituídos pelos prefabs 3D correspondentes; moldura, sombra e órbita permanecem na camada de UI quando aplicável.
14. Solicitar permissão Android ao entrar no fluxo de câmera e não gravar fotos.
15. Validar a interface no dispositivo Android; o Editor não substitui a verificação de Safe Area, câmera, legibilidade e toque.

## 12. Pendências de Design

Itens marcados como **A confirmar no Figma**:

- margens laterais, largura útil e sistema de colunas;
- regras exatas de responsividade, ancoragem, expansão e quebra de texto em outras proporções Android;
- peso e line height de todos os estilos tipográficos;
- atribuição dos tamanhos Fredoka `28`, `32` e `36 px` a cada nível de título;
- família, tamanho, peso e line height próprios da marca `SolarAR`;
- tamanhos exatos de labels, fatos e textos auxiliares;
- correspondência dos raios `20`, `24` e `28 px` a cada componente;
- parâmetros de sombra: cor, deslocamento, blur e opacidade;
- aplicação exata da escala de espaçamento a paddings, gaps e margens;
- tokens e aparência para erro, aviso, overlay, sombra, pressionado e desabilitado;
- largura e raio dos botões, dimensões dos componentes, headers, cards, moldura AR, navegação inferior e painéis;
- estados pressionado/selecionado dos controles e cards;
- duração da confirmação `Card reconhecido!`;
- biblioteca, espessura, tamanho e exportação dos ícones;
- destino exato de voltar por tela, comportamento do Android Back e persistência da aba;
- fluxo visual para permissão de câmera negada;
- destino do retorno da tela `Como usar`;
- critério técnico e limiar para identificar pouca luz;
- timeout e regra para sair de `Card perdido` e retornar à busca;
- quantidade e conteúdo dos fatos dos demais planetas;
- disponibilidade e conteúdo das narrações;
- comportamento de `Anterior` e `Próximo` nos extremos da lista;
- duração, curva e uso exato de animações e transições;
- comportamento com aumento de texto e limites de acessibilidade tipográfica.
