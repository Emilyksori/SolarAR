# Guia de desenvolvimento do SolarAR

## Objetivo do projeto

SolarAR é um aplicativo Android educacional de Realidade Aumentada (AR) para apoiar o aprendizado sobre os planetas do Sistema Solar. O aplicativo reconhece cards físicos por meio de rastreamento de imagens e exibe, sobre cada card, somente o planeta correspondente.

O público-alvo inclui crianças neurodivergentes. Por isso, toda decisão de produto deve favorecer uma experiência previsível, clara, acessível, com poucos estímulos simultâneos e controles fáceis de compreender.

Este é um projeto acadêmico. A prioridade é entregar um MVP simples, funcional, demonstrável e bem organizado, sem complexidade desnecessária.

## Tecnologias

- Unity como engine de desenvolvimento.
- C# como linguagem de programação.
- AR Foundation como camada de abstração para recursos de AR.
- Google ARCore como provedor de AR no Android.
- Image Tracking do AR Foundation para reconhecer os cards dos planetas.
- Android como plataforma-alvo do MVP.

As versões da Unity e dos pacotes devem ser definidas quando o projeto Unity for criado. Devem ser escolhidas versões estáveis, compatíveis entre si e adequadas ao Android. Não adicionar dependências sem necessidade clara.

## Escopo do MVP

O MVP deve incluir:

1. Reconhecimento de um conjunto definido de cards físicos de planetas.
2. Associação única entre cada imagem de referência e seu planeta.
3. Exibição de somente um modelo correspondente sobre cada card reconhecido.
4. Posicionamento estável do planeta em relação ao card.
5. Rotação do planeta por gesto do usuário.
6. Redimensionamento do planeta por gesto de pinça, com limites mínimos e máximos.
7. Conteúdo educacional curto e legível para cada planeta.
8. Interface simples para abrir, fechar ou alternar a visualização do conteúdo educacional.
9. Feedback básico quando a câmera procura ou reconhece um card.
10. Execução em um dispositivo Android compatível com ARCore.

Ficam fora do MVP, salvo decisão explícita futura:

- autenticação e contas de usuário;
- serviços online, banco de dados remoto ou sincronização em nuvem;
- multiplayer;
- gamificação complexa;
- reconhecimento de objetos 3D;
- suporte a iOS;
- animações, efeitos visuais ou sistemas de áudio complexos;
- arquitetura genérica para casos que ainda não existem.

## Arquitetura

O código e os recursos próprios do projeto devem ser separados por responsabilidade:

- **Core**: inicialização, configurações compartilhadas, tipos comuns e coordenação geral do fluxo do aplicativo.
- **AR**: sessão de AR, rastreamento de imagens, eventos de detecção e vínculo entre a imagem reconhecida e o conteúdo exibido.
- **Planets**: dados, modelos, prefabs, materiais e comportamento específico dos planetas.
- **Interaction**: entrada por toque, rotação e redimensionamento dos objetos.
- **Education**: textos e demais conteúdos educacionais, além da lógica para selecioná-los.
- **UI**: telas, painéis, mensagens de estado e controles visuais.
- **Audio**: efeitos e narrações, caso sejam incluídos no escopo aprovado.

Manter as dependências entre módulos simples e explícitas. A camada de UI não deve implementar rastreamento de imagens, e a camada de AR não deve conter textos educacionais. Dados de planetas devem ser configuráveis, preferencialmente por `ScriptableObject`, quando isso reduzir duplicação sem aumentar a complexidade.

Cada imagem de referência deve possuir um identificador estável. Esse identificador deve ser associado de forma explícita a apenas um planeta. Não depender de buscas frágeis por nome de GameObject quando uma referência serializada ou um mapeamento configurável resolver o problema.

### Componentes planejados

Os componentes abaixo representam a arquitetura planejada do SolarAR:

- **Core**
  - `SolarARManager`: coordenação do fluxo principal do aplicativo.
  - `EventSystem`: comunicação desacoplada entre os principais sistemas, quando necessária.
  - `AppState`: representação do estado atual do aplicativo.
- **AR**
  - `ARImageTrackingManager`: gerenciamento do rastreamento de imagens.
  - `TrackedImageHandler`: tratamento do ciclo de vida de cada imagem rastreada.
  - `ReferenceImageLibrary`: configuração ou acesso às imagens de referência e suas associações.
- **Planets**
  - `Planet`: representação dos dados e características de um planeta.
  - `PlanetController`: coordenação do comportamento visual do planeta.
  - `RotationController`: controle de rotação própria, quando necessário.
  - `OrbitController`: possibilidade arquitetural para movimento orbital futuro.
  - `PlanetRegistry`: associação e consulta dos planetas disponíveis.
- **Interaction**
  - `PlanetSelection`: identificação do planeta selecionado pelo usuário.
  - `GestureController`: interpretação e coordenação dos gestos de toque.
  - `ScaleController`: aplicação do redimensionamento com limites seguros.
  - `RotationInteraction`: aplicação da rotação feita pelo usuário.
- **Education**
  - `PlanetContent`: conteúdo educacional associado a um planeta.
  - `PlanetFacts`: fatos curtos e estruturados sobre cada planeta.
  - `EducationalContentManager`: seleção e apresentação do conteúdo correspondente.
- **UI**
  - `PlanetHUD`: informações e controles exibidos durante a visualização de um planeta.
  - `MainMenu`: entrada e navegação inicial do aplicativo.
  - `Instructions`: orientações simples para uso dos cards e gestos.
  - `ARFeedback`: mensagens sobre busca, reconhecimento e perda de tracking.

Essa lista é um guia de responsabilidades, não uma exigência de implementação imediata. Criar cada componente somente quando ele for necessário para uma etapa real do MVP. Não gerar classes vazias apenas para reproduzir a arquitetura planejada; componentes podem ser ajustados, combinados ou adiados quando uma solução mais simples atender melhor ao requisito atual.

`OrbitController` é somente uma possibilidade arquitetural. Movimento orbital não é prioridade para a primeira versão do MVP e não deve ser implementado sem um requisito explícito.

## Organização de pastas

Quando o projeto Unity for criado, usar como base:

```text
Assets/
  SolarAR/
    Core/
      Scripts/
    AR/
      Scripts/
      ReferenceImages/
    Planets/
      Scripts/
      Data/
      Prefabs/
      Models/
      Materials/
      Textures/
    Interaction/
      Scripts/
    Education/
      Scripts/
      Data/
    UI/
      Scripts/
      Prefabs/
    Audio/
      Scripts/
      Clips/
    Scenes/
    Tests/
```

- Colocar todos os recursos próprios em `Assets/SolarAR` para separá-los de pacotes e recursos de terceiros.
- Manter scripts no módulo ao qual pertencem.
- Não criar pastas vazias antecipadamente; criá-las conforme o conteúdo for implementado.
- Evitar pastas genéricas como `Misc`, `Other` ou `Temp`.
- Preservar e versionar os arquivos `.meta` gerados pela Unity junto com seus respectivos arquivos.
- Não versionar pastas geradas localmente pela Unity, como `Library`, `Temp`, `Logs` e `Obj`.

## Convenções de código C#

- Escrever nomes de tipos, métodos, propriedades e membros públicos em `PascalCase`.
- Escrever variáveis locais e parâmetros em `camelCase`.
- Usar `_camelCase` para campos privados.
- Usar um tipo público principal por arquivo e manter o nome do arquivo igual ao nome do tipo.
- Usar namespaces iniciados por `SolarAR` e organizados por módulo, como `SolarAR.AR` e `SolarAR.Interaction`.
- Declarar campos do Inspector como `[SerializeField] private` em vez de torná-los públicos sem necessidade.
- Preferir composição a heranças profundas.
- Manter `MonoBehaviour` pequeno e com uma responsabilidade clara.
- Separar dados configuráveis da lógica quando isso simplificar manutenção e testes.
- Evitar métodos longos, estado global, singletons desnecessários e referências obtidas repetidamente com `Find`.
- Remover código morto e não deixar warnings conhecidos sem justificativa.
- Escrever comentários somente quando explicarem intenção, restrição ou decisão que o próprio código não deixa clara.
- Usar nomes claros em inglês no código e nos arquivos técnicos. O conteúdo exibido ao usuário pode ser escrito em português.
- Não adicionar abstrações, interfaces ou padrões de projeto sem uma necessidade concreta no MVP.

## Experiência e acessibilidade

- Apresentar instruções curtas, diretas e consistentes.
- Evitar excesso de movimento, som, cores piscantes ou informações simultâneas.
- Usar boa legibilidade, contraste adequado e áreas de toque confortáveis.
- Não depender somente de cor ou áudio para comunicar uma ação ou estado.
- Permitir que o usuário controle a abertura do conteúdo e a reprodução de áudio.
- Evitar animações automáticas intensas; quando usadas, devem ser suaves e ter propósito educacional.
- Validar decisões de acessibilidade com pessoas qualificadas e, quando possível, com representantes do público-alvo. Não presumir que uma única solução atende todas as pessoas neurodivergentes.

## Regras para alterações futuras

Antes de implementar uma mudança:

1. Confirmar que ela contribui para o MVP ou para um requisito aprovado.
2. Verificar a compatibilidade com Unity, AR Foundation, ARCore e Android.
3. Inspecionar a estrutura e as convenções já existentes antes de criar novos componentes.
4. Reutilizar soluções existentes quando elas forem claras e adequadas.
5. Manter cada alteração pequena, focada e fácil de revisar.

Durante e depois da implementação:

- Não instalar ou atualizar pacotes sem justificar a necessidade e verificar a compatibilidade das versões.
- Não incluir segredos, credenciais, arquivos de build ou arquivos gerados pela Unity no repositório.
- Não alterar configurações globais do projeto sem documentar o motivo e o impacto.
- Não renomear ou mover assets fora da Unity quando isso puder quebrar referências e arquivos `.meta`.
- Tratar perda de tracking, troca de card e remoção de imagem rastreada sem deixar planetas incorretos ou duplicados na cena.
- Garantir que cada card mostre somente o planeta associado a ele.
- Limitar escala e outros gestos para evitar estados inutilizáveis.
- Testar mudanças de AR em dispositivo Android compatível; testes no Editor não substituem a validação no aparelho.
- Adicionar testes automatizados para lógica C# independente da Unity ou de hardware quando isso for prático.
- Atualizar a documentação quando uma decisão de arquitetura, configuração ou escopo mudar.
- Preservar alterações do usuário que não façam parte da tarefa atual.

## Estratégia Git

O projeto deve utilizar uma estratégia simples, baseada em branches curtas e adequada a um projeto acadêmico pequeno. Não adotar um Git Flow completo nem criar branches permanentes como `develop`, `release` ou `hotfix`, pois isso adicionaria complexidade desnecessária ao SolarAR.

### Estratégia de branches

A branch `main` representa o estado estável do projeto e deve permanecer funcional e demonstrável.

Não desenvolver novas funcionalidades diretamente na `main`, salvo quando houver uma instrução explícita para isso. Antes de começar uma tarefa de desenvolvimento, verificar a branch atual. Novas alterações devem normalmente partir da versão mais recente da `main`.

Fluxo esperado:

```bash
git switch main
git pull origin main
git switch -c <tipo>/<nome-da-tarefa>
```

Esses comandos, respectivamente, mudam para a branch estável, obtêm sua versão mais recente do repositório remoto e criam uma branch dedicada à nova tarefa. Antes de executá-los, preservar alterações locais em andamento e confirmar que a troca de branch é segura.

#### Prefixos permitidos

Usar:

- `feature/` para novas funcionalidades.
- `fix/` para correções de bugs.
- `docs/` para alterações exclusivamente de documentação.
- `chore/` para configuração, dependências, organização e manutenção.
- `refactor/` para refatorações que não alterem o comportamento esperado.
- `test/` para criação ou alteração de testes.

#### Convenção para nomes de branches

Os nomes das branches devem:

- ser escritos em inglês;
- utilizar letras minúsculas;
- separar palavras com hífen;
- descrever claramente o objetivo da tarefa;
- evitar nomes genéricos ou pessoais.

Exemplos:

```text
feature/ar-image-tracking
feature/earth-planet
feature/planet-gestures
feature/educational-content

fix/card-tracking-duplicate
fix/planet-scale-limit

docs/update-readme
docs/update-architecture

chore/configure-ar-foundation
chore/configure-android-build

refactor/planet-controller

test/planet-data
```

#### Regras para branches

- Uma branch deve representar apenas uma tarefa ou objetivo principal.
- Não misturar funcionalidades independentes na mesma branch.
- Manter branches pequenas e de curta duração.
- Antes de iniciar uma branch, atualizar a `main`.
- Não executar `force push` na `main`.
- Não apagar ou sobrescrever alterações do usuário.
- Antes de integrar uma branch, verificar se o projeto continua compilando e funcionando.
- Alterações relacionadas à Unity devem preservar corretamente os arquivos `.meta`.
- Não resolver conflitos de arquivos ou assets da Unity automaticamente sem entender o impacto.
- Depois que uma branch for concluída, realizar uma revisão antes de integrá-la à `main`.
- Após o merge e a confirmação de que a integração foi concluída corretamente, a branch pode ser removida.

### Convenção de commits

Utilizar mensagens de commit baseadas no padrão Conventional Commits simplificado:

```text
tipo: descrição curta
```

Tipos recomendados:

- `feat:` nova funcionalidade.
- `fix:` correção de bug.
- `docs:` documentação.
- `chore:` configuração, dependências ou manutenção.
- `refactor:` refatoração sem alteração funcional.
- `test:` testes.
- `style:` formatação ou alterações que não modificam o comportamento.

Exemplos:

```text
feat: add AR image tracking
feat: add Earth planet prefab
fix: prevent duplicate planet instances
docs: update project architecture
chore: configure AR Foundation
chore: configure Android build
refactor: simplify planet controller
test: add planet data tests
```

Regras para commits:

- Escrever mensagens de commit em inglês.
- Usar uma descrição curta e objetiva.
- Preferir verbos no imperativo ou descrições diretas.
- Fazer commits pequenos e focados.
- Cada commit deve representar uma alteração lógica.
- Evitar mensagens vagas como `changes`, `update`, `fix`, `new stuff` ou similares.
- Não incluir arquivos gerados, caches, builds ou arquivos ignorados pela configuração Git.
- Antes de considerar uma tarefa concluída, revisar `git status` e `git diff`.
- Não criar commits automaticamente, salvo quando solicitado explicitamente pelo usuário.
- Não executar `git push` automaticamente, salvo quando solicitado explicitamente pelo usuário.

## Princípio de decisão

Sempre priorizar a solução mais simples que cumpra o requisito e seja compatível com Unity, AR Foundation, ARCore e Android. Para o MVP, clareza, estabilidade e facilidade de demonstração são mais importantes do que flexibilidade especulativa, efeitos avançados ou uma arquitetura excessivamente genérica.

## Working with Codex

- Fazer mudanças pequenas, focadas e incrementais.
- Antes de implementar uma funcionalidade grande, apresentar resumidamente o plano e os principais arquivos que poderão ser afetados.
- Depois de cada alteração, explicar quais arquivos foram modificados e por quê.
- Sempre que possível, explicar conceitos de Unity e C# de maneira simples, considerando que a desenvolvedora está aprendendo.
- Não implementar funcionalidades que não tenham sido solicitadas.
- Não executar alterações destrutivas sem explicar previamente a ação, o impacto e o motivo.
- Antes de considerar uma tarefa concluída, verificar `git status` e `git diff`, procurar alterações inesperadas e avaliar possíveis problemas relevantes.

### Regras específicas para trabalho com Codex e Git

1. Antes de implementar uma tarefa, verificar a branch atual.
2. Se a branch atual for `main` e a tarefa envolver código, configuração significativa ou uma nova funcionalidade, avisar que uma branch dedicada deve ser criada antes de continuar.
3. Sugerir um nome de branch adequado seguindo a convenção do projeto.
4. Não criar, trocar, fazer merge ou apagar branches sem solicitação ou aprovação explícita.
5. Não realizar `commit`, `push`, `merge`, `rebase`, `reset`, checkout destrutivo ou `force push` sem solicitação explícita.
6. O Codex pode sugerir comandos Git, mas deve explicar resumidamente o propósito deles ao trabalhar com uma desenvolvedora que está aprendendo.
7. Nunca assumir que uma alteração deve ser enviada diretamente para a `main`.
8. Ao terminar uma tarefa de desenvolvimento, apresentar:
   - arquivos modificados;
   - resumo das alterações;
   - testes ou verificações executados;
   - possíveis pendências;
   - sugestão de mensagem de commit apropriada.
