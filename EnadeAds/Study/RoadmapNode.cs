namespace EnadeAds.Study;

public enum NodeKind
{
    Subject = 0,
    Topic = 1,
    Checkpoint = 2,
}

public enum StudyBlock
{
    Fundamentals = 0,
    DataAndInfrastructure = 1,
    SoftwareEngineering = 2,
    ProfessionalContext = 3,
    GeneralEducation = 4,

    // ENEM: as quatro áreas da prova e a redação.
    Languages = 5,
    Humanities = 6,
    NaturalSciences = 7,
    Mathematics = 8,
    Essay = 9,
}

public enum NodeStatus
{
    Done = 1,
    InProgress = 2,
    Skipped = 3,
}

/// <summary>
/// Nó do roadmap. Matérias e checkpoints formam a espinha central (sem pai);
/// tópicos são os assuntos de uma matéria e apontam para ela em <see cref="ParentId"/>.
/// </summary>
public class RoadmapNode
{
    public int Id { get; set; }
    public int TrackId { get; set; } = Tracks.Default;
    public required string Slug { get; set; }
    public required string Title { get; set; }
    public required string Summary { get; set; }
    public NodeKind Kind { get; set; }
    public StudyBlock Block { get; set; }

    /// <summary>Posição na espinha (matérias e checkpoints) ou dentro da matéria (tópicos).</summary>
    public int Order { get; set; }

    public int? ParentId { get; set; }
    public RoadmapNode? Parent { get; set; }
    public List<RoadmapNode> Topics { get; set; } = [];
}

public class RoadmapProgress
{
    public required string UserId { get; set; }
    public int NodeId { get; set; }
    public NodeStatus Status { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public static class StudyBlockNames
{
    public static string Name(this StudyBlock block) => block switch
    {
        StudyBlock.Fundamentals => "Fundamentos",
        StudyBlock.DataAndInfrastructure => "Dados e infraestrutura",
        StudyBlock.SoftwareEngineering => "Engenharia de software",
        StudyBlock.ProfessionalContext => "Contexto profissional",
        StudyBlock.GeneralEducation => "Formação geral",
        StudyBlock.Languages => "Linguagens",
        StudyBlock.Humanities => "Ciências humanas",
        StudyBlock.NaturalSciences => "Ciências da natureza",
        StudyBlock.Mathematics => "Matemática",
        StudyBlock.Essay => "Redação",
        _ => block.ToString(),
    };

    /// <summary>
    /// Classe CSS da cor do bloco (app.css): a cor de uma matéria é a do bloco dela, em todo o app.
    /// As trilhas reaproveitam as cinco cores; dentro de uma trilha, cada bloco tem a sua.
    /// </summary>
    public static string ColorClass(this StudyBlock block) => block switch
    {
        StudyBlock.Fundamentals or StudyBlock.Essay => "block-fund",
        StudyBlock.DataAndInfrastructure or StudyBlock.Mathematics => "block-data",
        StudyBlock.SoftwareEngineering or StudyBlock.NaturalSciences => "block-soft",
        StudyBlock.ProfessionalContext or StudyBlock.Languages => "block-prof",
        _ => "block-gen",
    };
}

public static class NodeStatusNames
{
    public static string Name(this NodeStatus status) => status switch
    {
        NodeStatus.Done => "Concluído",
        NodeStatus.InProgress => "Estudando",
        NodeStatus.Skipped => "Pulei",
        _ => status.ToString(),
    };
}
