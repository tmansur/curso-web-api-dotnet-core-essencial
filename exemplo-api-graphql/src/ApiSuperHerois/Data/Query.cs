using ApiSuperHerois.Entities;

namespace ApiSuperHerois.Data;

public class Query
{
  /// <summary>
  /// Define uma consulta graphql para recuperar dados de super herois
  /// </summary>
  /// <param name="context"></param>
  /// <returns></returns>
  [UseProjection]
  [UseFiltering]
  [UseSorting]
  public IQueryable<SuperHeroi> GetSuperherois([Service] AppDbContext context) =>
      context.SuperHerois;
}

/*

Exemplos de consultas que podem ser feitas:

query{
  superherois{
    nome
    descricao
  }
}

query{
  superherois{
    nome
    descricao
    superPoderes{
      super_Poder
    }
  }
}

query{
  superherois{
    nome
    descricao
    superPoderes(order: {super_Poder:ASC}){
      super_Poder
      descricao
    }
    filmes{
      titulo
    }
  }
}

query{
  superherois(where: {nome: {eq: "Batman"}}){
    descricao
    filmes{
      titulo
    }
  }
}

*/