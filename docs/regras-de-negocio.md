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

## Acesso ao conteúdo

- **Conteúdo é aberto:** roadmap, matérias, enunciados das questões, desafios e avisos podem ser vistos sem login.
- **Responder exige login:** marcar uma alternativa, enviar uma resposta discursiva e ver o gabarito ou o comentário exigem uma conta.
- O progresso (sessões do roadmap concluídas, questões respondidas e acertos) é salvo na conta de cada aluno e só ele vê o próprio progresso.

## Roadmap

- O roadmap mostra as 20 matérias da portaria e a Formação Geral, com seus assuntos e um checkpoint ao fim de cada bloco.
- Cada aluno marca matérias e assuntos como **Concluído**, **Estudando** ou **Pulei**, e pode limpar o status.
- A porcentagem conta só matérias e assuntos concluídos; checkpoints são marcos e não entram na conta.
- A contagem regressiva usa a data da prova configurada (`Enade:ExamDate`) e o fuso de São Paulo.

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
