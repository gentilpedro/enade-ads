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
        _ => block.ToString(),
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
