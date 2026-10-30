# Regras de negócio

Este documento reúne as decisões de produto da plataforma. Toda mudança de regra passa por aqui no mesmo PR que muda o código.

## Perfis

| Perfil | Quem é | O que pode fazer |
|---|---|---|
| Visitante | Quem não entrou na conta | Ver o roadmap, as matérias, as questões (sem gabarito), os desafios e os avisos |
| Aluno | Toda conta cadastrada | Tudo do visitante, mais responder questões, ver o gabarito e salvar o progresso |
| ProfessorPendente | Pediu acesso de professor e aguarda aprovação | O mesmo que o aluno |
| Professor | Aprovado por um administrador | Tudo do aluno, mais publicar e editar questões, desafios semanais e avisos |
| Admin | E-mail listado na configuração `Admin:Emails` | Tudo do professor, mais aprovar ou recusar professores |

Uma conta pode ter mais de um perfil ao mesmo tempo. Por exemplo, um professor aprovado também é aluno.

## Cadastro

- Toda conta nova recebe o perfil **Aluno**.
- O formulário tem a caixa **"Sou professor"**. Ao marcá-la, aparece um texto explicando que o acesso depende de aprovação.
- **"Sou professor" marcado e e-mail institucional:** a conta recebe **Aluno + ProfessorPendente**.
- **"Sou professor" marcado e e-mail não institucional:** o cadastro é recusado e nenhuma conta é criada.
- **"Sou professor" desmarcado:** a conta é só de **Aluno**, mesmo com e-mail institucional.
- O e-mail é institucional quando tem `edu` como parte do domínio, depois do nome da instituição: `nome@faculdade.edu.br` e `nome@mit.edu` são; `nome@educacao.com.br` não é.
- A conta só entra depois de confirmar o e-mail pelo link enviado. É isso que prova que o professor é dono do endereço `.edu`.
- E-mails listados em `Admin:Emails` recebem também o perfil **Admin** ao se cadastrar.

### Por que o professor fica pendente

Alunos também têm e-mail institucional, muitas vezes no mesmo domínio dos professores. Sem aprovação, qualquer aluno marcaria a caixa e passaria a publicar questões e ver gabaritos como professor.

## Aprovação de professores

- Só **Admin** vê a lista de pedidos pendentes.
- **Aprovar:** a conta perde ProfessorPendente e ganha **Professor**.
- **Recusar:** a conta perde ProfessorPendente e continua como **Aluno**.
- Depois da decisão, a sessão da pessoa é renovada em até 1 minuto e ela precisa entrar de novo para o novo perfil valer.

## Gestão de contas

- Só **Admin** abre a tela *Contas*: busca por nome ou e-mail e filtro por perfil (alunos, professores pendentes, professores, administradores, bloqueadas).
- O admin pode **revogar o perfil de Professor** (a pessoa continua Aluno), **remover o perfil de Admin** de outra pessoa e **bloquear ou desbloquear** uma conta. Conta bloqueada não consegue entrar.
- Toda mudança vale em até 1 minuto: a sessão da pessoa é renovada e ela precisa entrar de novo.
- Um admin **não altera a própria conta** por essa tela, para não se trancar fora nem deixar a plataforma sem admin por engano.
- Novos admins continuam vindo só da configuração `Admin:Emails`.

## Privacidade (LGPD)

- O cadastro exige aceitar os **Termos de uso** e a **Política de privacidade**. A data e a versão aceitas ficam guardadas na conta.
- Os textos ficam em `/termos` e `/privacidade`, com link no rodapé de todas as páginas. O responsável pelos dados e o e-mail de contato vêm da configuração (`Privacy:ControllerName`, `Privacy:ContactEmail`), porque cada instalação tem o seu responsável.
- **Baixar meus dados** (Minha conta → Dados pessoais) entrega um arquivo com a conta, o progresso no roadmap, as respostas, os simulados, os desafios e os comentários.
- **Excluir a conta** apaga a conta, o progresso, as respostas objetivas e discursivas, os simulados e os envios de desafio. Os **comentários no blog continuam, sem o nome** ("Participante que excluiu a conta"). Questões, desafios e posts que um professor publicou continuam, sem autor.
- Registros técnicos de acesso ficam até 31 dias; cópias de segurança, até 30 dias.
- O conteúdo publicado por professores segue a licença Creative Commons BY-SA 4.0; o código, a licença MIT.

## Proteção contra abuso

- Os formulários de entrar, criar conta, esqueci a senha, reenviar confirmação, criar nova senha e códigos de 2FA aceitam no máximo **10 envios por minuto por IP**. Acima disso, a pessoa vê "Muitas tentativas em pouco tempo. Aguarde um minuto e tente de novo."
- **5 senhas erradas seguidas** bloqueiam a conta por **15 minutos**. O admin pode desbloquear antes pela tela *Contas*.
- Todo texto enviado tem limite de tamanho: comentário 2.000 caracteres, resposta discursiva 5.000, enunciado 8.000, post 20.000.
- O site envia cabeçalhos de segurança: política de conteúdo (CSP), proibição de ser exibido dentro de outro site (frame) e de adivinhar tipo de arquivo.

## Acesso ao conteúdo

- **Conteúdo é aberto:** roadmap, matérias, enunciados das questões, desafios e avisos podem ser vistos sem login.
- **Responder exige login:** marcar uma alternativa, enviar uma resposta discursiva e ver o gabarito ou o comentário exigem uma conta.
- O progresso (sessões do roadmap concluídas, questões respondidas e acertos) é salvo na conta de cada aluno e só ele vê o próprio progresso.

## Roadmap

- O roadmap mostra as 20 matérias da portaria e a Formação Geral, com seus assuntos e um checkpoint ao fim de cada bloco.
- Cada aluno marca matérias e assuntos como **Concluído**, **Estudando** ou **Pulei**, e pode limpar o status.
- A porcentagem conta só matérias e assuntos concluídos; checkpoints são marcos e não entram na conta.
- A contagem regressiva usa a data da prova definida pelo **admin** na tela *Data da prova* (se nunca foi definida, vale a configuração `Enade:ExamDate`) e o fuso de São Paulo. A data não pode estar no passado.

### Edição do roadmap

- **Professor** e **Admin** editam o roadmap pela tela (link *Editar* no painel lateral e na página da matéria). Diferente de questões e posts, o roadmap é de todos os professores: qualquer professor edita qualquer tópico.
- Das **matérias** dá para editar título e explicação e reordenar dentro do bloco. Elas **não podem ser removidas**, porque vêm da Portaria Inep nº 169/2026.
- **Assuntos** podem ser criados (entram no fim da matéria), editados, reordenados dentro da matéria e removidos.
- **Checkpoints** podem ser criados logo depois de uma matéria, editados, reordenados dentro do bloco e removidos.
- O endereço de um tópico (`/roadmap?topico=...`) não muda quando o título é editado, para links compartilhados continuarem valendo.
- Remover um assunto ou checkpoint apaga só o status que os alunos marcaram nele; o resto do progresso não muda.

## Simulados

- O aluno logado monta um simulado com questões do banco em três tamanhos, todos com a proporção da prova (Portaria Inep nº 169/2026): **Curto** (3 de formação geral + 7 específicas + 1 discursiva), **Médio** (7 + 15 + 1) e **Completo** (15 + 30 + 1).
- O tempo é proporcional ao da prova: 4 horas para 46 questões (o curto fica com cerca de 57 minutos). Mínimo de 10 minutos.
- Se o banco ainda não tem questões suficientes, o simulado usa as que existem (completando formação geral com específicas e vice-versa) e avisa antes de começar quantas questões e quanto tempo terá.
- O **prazo é gravado no servidor** quando o simulado começa. Recarregar a página ou trocar de aparelho não reinicia o relógio.
- O aluno pode salvar as respostas no meio do caminho e entregar quando quiser. Quando o tempo acaba, o simulado é **entregue automaticamente** com o que estava salvo. Há 1 minuto de tolerância para o envio feito no último segundo.
- Só existe **um simulado em andamento por aluno**. Ele precisa entregar (ou esperar o tempo acabar) para começar outro.
- O **gabarito comentado e o padrão de resposta só aparecem depois da entrega**. Questão em branco conta como errada.
- Depois da entrega, as respostas objetivas entram no painel de desempenho e no caderno de erros, e a discursiva vira uma resposta salva da questão.
- O simulado é do aluno: ninguém mais vê o que ele respondeu.

## Painel de desempenho

- A página **Meu desempenho** é só do próprio aluno; ninguém mais vê os números dele.
- Em cada questão objetiva vale a **resposta mais recente**: errou e depois acertou, conta como acerto.
- A taxa de acerto aparece no total, por bloco e por matéria. **Revise primeiro** lista as 3 matérias com menor acerto entre as que o aluno já respondeu.
- O **caderno de erros** lista as questões em que a última resposta foi errada, com link para refazer. Ao acertar, a questão sai da lista. Questões arquivadas não aparecem.
- Respostas de um desafio **ainda aberto** não entram no painel, para não revelar quais questões estão erradas antes do gabarito sair. Depois que o desafio encerra, elas passam a contar.
- Discursivas não têm nota automática: o painel mostra só quantas o aluno respondeu.
- O painel também mostra o progresso no roadmap e a nota de cada desafio de que o aluno participou.

## Blog de atualizações

- O blog reúne **atualizações sobre o ENADE** em quatro categorias: *Datas e prazos*, *Portarias e editais*, *Dicas de estudo* e *Avisos da turma*.
- **Professor** e **Admin** escrevem posts com título, resumo, texto, categoria e, se houver, o link da **fonte oficial** (sempre https).
- O post pode ficar como **rascunho** (só o autor e os admins veem) até ser publicado. Professor edita só os próprios posts; admin edita qualquer um.
- O endereço do post (`/blog/titulo-do-post`) sai do título e **não muda depois de publicado**, para links compartilhados continuarem valendo.
- **Fixar** coloca o post no topo do blog e da página inicial. Só um post fica fixado por vez.
- **Arquivar** tira o post do blog, da página inicial e do feed. Publicar de novo traz ele de volta.
- Qualquer pessoa lê posts e comentários. **Comentar exige login.**
- O autor edita e exclui o próprio comentário; professor e admin excluem qualquer comentário (moderação).
- O texto do post aceita a marcação mínima (parágrafos, listas, negrito, links e imagens por https). Comentários são texto puro.
- O feed RSS (`/blog/feed.xml`) traz os 20 posts publicados mais recentes.

## Desafios semanais

- **Professor** e **Admin** criam desafios: título, descrição, primeiro e último dia, e de 1 a 30 questões **objetivas** publicadas do banco.
- O desafio fica **agendado** antes do primeiro dia (só quem edita vê), **aberto** do primeiro ao último dia (no fuso de São Paulo) e **encerrado** depois, ou antes se o professor encerrar.
- Visitante vê as questões de um desafio aberto, sem gabarito. Para participar é preciso entrar.
- Cada aluno envia **uma vez** por desafio e precisa responder todas as questões. Logo depois vê a própria nota (quantas acertou), mas **não o gabarito**.
- Quando o desafio encerra, todos veem o gabarito comentado e o desempenho da turma: quantos participaram e a taxa de acerto de cada questão. Não há ranking com nomes.
- As questões de um desafio não mudam depois que algum aluno enviou respostas. Título, descrição e datas ainda podem ser ajustados.
- As respostas dadas no desafio contam só no desafio: não aparecem como tentativa na página da questão nem liberam o gabarito lá.
- **Limite conhecido:** as questões do desafio são do banco público. Quem procurar a mesma questão no banco consegue responder lá e ver o gabarito antes do desafio encerrar.

## Questões

- Visitante vê o enunciado e as alternativas, mas nunca a alternativa correta nem os comentários: eles não vão para a página.
- Ao responder, o aluno vê se acertou, qual é a correta e o comentário de **todas** as alternativas.
- Cada resposta fica registrada como uma tentativa. O aluno pode responder de novo quantas vezes quiser; a lista de questões mostra o resultado da tentativa mais recente.
- Toda questão informa a fonte: prova e ano do Inep, ou autoria própria.

### Questões discursivas

- A discursiva tem enunciado, **padrão de resposta** (o que uma boa resposta deve conter) e um tamanho sugerido em linhas.
- Visitante vê só o enunciado. O aluno logado escreve a resposta e **só depois de enviar** vê o padrão de resposta, lado a lado com o que escreveu.
- A resposta não recebe nota automática: o aluno compara com o padrão. Na lista, a questão aparece como **Respondida**.
- Só o próprio aluno vê o que escreveu. Ele pode escrever de novo quantas vezes quiser.

### Publicação de questões

- **Professor** e **Admin** criam questões. Professor edita e arquiva só as que ele criou; Admin edita e arquiva qualquer uma.
- Toda questão tem matéria, enunciado e fonte. A objetiva tem 5 alternativas com texto e comentário, e exatamente uma correta; a discursiva tem padrão de resposta. A validação acontece no servidor.
- O tipo (objetiva ou discursiva) é escolhido ao criar e não muda depois.
- Antes de publicar, dá para pré-visualizar a questão como o aluno vai ver.
- **Arquivar** tira a questão das listas e da página pública. As respostas que os alunos já deram continuam no histórico deles.
- Editar uma questão não muda o resultado das respostas antigas: cada tentativa guarda se acertou no momento em que respondeu.
- O enunciado aceita só uma marcação mínima (parágrafos, negrito, código e imagens por https). Nenhum HTML digitado chega à página.
- Se o professor apagar a conta, as questões que ele publicou continuam na plataforma, sem autor.
