using System.Collections.Generic;

public class PaginatedHighscoreResponseDTO 
{
    public int total;
    public int limit;
    public string next;
    public string previous;
    public List<HighScoreResultsDTO> results;

    public PaginatedHighscoreResponseDTO(int total, int limit, string next, string previous, List<HighScoreResultsDTO> results)
    {
        this.total = total;
        this.limit = limit;
        this.next = next;
        this.previous = previous;
        this.results = results;
    }

    public int getTotal() { return this.total; }
    public int getLimit() { return this.limit; }
    public string getNext() { return this.next; }
    public string getPrevious() { return this.previous; }
    public List<HighScoreResultsDTO> getResults() { return this.results; }

    public void setTotal(int total) { this.total = total; }
    public void setLimit(int limit) { this.limit = limit; }
    public void setNext(string next) { this.next = next; }
    public void setPrevious(string previous) { this.previous = previous; }
    public void setResults(List<HighScoreResultsDTO> results) { this.results = results; }
}
