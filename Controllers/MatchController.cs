using Microsoft.AspNetCore.Mvc;
using webapplication.Models;
using System.Collections.Generic;
using System.Linq;

// Queue Node sınıfı - Queue'daki her bir elemanı temsil eder
public class MatchQueueNode
{
    public Match Data { get; set; }
    public MatchQueueNode Next { get; set; }

    public MatchQueueNode(Match match)
    {
        Data = match;
        Next = null;
    }
}

// Custom Queue sınıfı
public class MatchQueue
{
    private MatchQueueNode _front;  // Queue'nun başı
    private MatchQueueNode _rear;   // Queue'nun sonu
    private int _count;             // Queue'daki eleman sayısı

    public MatchQueue()
    {
        _front = null;
        _rear = null;
        _count = 0;
    }

    // Queue'ya yeni bir eleman ekle (enqueue)
    public void Enqueue(Match match)
    {
        MatchQueueNode newNode = new MatchQueueNode(match);

        // Queue boş ise
        if (_rear == null)
        {
            _front = newNode;
            _rear = newNode;
        }
        else
        {
            // Yeni node'u sona ekle
            _rear.Next = newNode;
            _rear = newNode;
        }

        _count++;
    }

    // Queue'dan bir eleman çıkar (dequeue)
    public Match Dequeue()
    {
        // Queue boş ise null döndür
        if (_front == null)
            return null;

        // Front node'u geçici olarak sakla
        MatchQueueNode temp = _front;
        Match match = temp.Data;

        // Front'u bir sonraki node'a taşı
        _front = _front.Next;

        // Queue boşaldıysa rear'ı da null yap
        if (_front == null)
            _rear = null;

        _count--;
        return match;
    }

    // Queue'nun başındaki elemanı çıkarmadan döndür (peek)
    public Match Peek()
    {
        if (_front == null)
            return null;

        return _front.Data;
    }

    // Queue'daki eleman sayısını döndür
    public int Count()
    {
        return _count;
    }

    // Queue'yu temizle
    public void Clear()
    {
        _front = null;
        _rear = null;
        _count = 0;
    }

    // Queue'daki tüm elemanları liste olarak döndür
    public List<Match> ToList()
    {
        List<Match> result = new List<Match>();
        MatchQueueNode current = _front;

        while (current != null)
        {
            result.Add(current.Data);
            current = current.Next;
        }

        return result;
    }
}

[Route("api/[controller]")]
[ApiController]
public class MatchController : ControllerBase
{
    private readonly DataContext _context;
    private static MatchQueue _matchQueue = new MatchQueue();

    public MatchController(DataContext context)
    {
        _context = context;

        // Queue'yu ilk kez doldur (eğer boşsa)
        if (_matchQueue.Count() == 0)
        {
            RefreshMatchQueue();
        }
    }

    // Queue'yu veritabanından yeniden doldur
    private void RefreshMatchQueue()
    {
        // Önce queue'yu temizle
        _matchQueue.Clear();

        // Henüz tamamlanmamış ve iptal edilmemiş maçları al (status 0 veya 1 olanlar)
        var pendingMatches = _context.Matches
            .Where(m => m.Status == 0 || m.Status == 1)
            .OrderBy(m => m.MatchDate)
            .ToList();

        // Maçları queue'ya ekle
        foreach (var match in pendingMatches)
        {
            _matchQueue.Enqueue(match);
        }
    }

    [HttpGet]
    public IActionResult GetMatches()
    {
        // Veritabanından tüm maçları al
        var matches = _context.Matches.ToList();

        // Tüm takımları alıp ID'lerine göre sözlük haline getir
        var teams = _context.Teams
            .ToDictionary(t => t.Id, t => t.Name);

        // Maçlara göre takımları eşleştir
        var result = matches.Select(m => new
        {
            m.Id,
            HomeTeamName = m.HomeTeamName,
            AwayTeamName = m.AwayTeamName,
            HomeScore = m.HomeScore,
            AwayScore = m.AwayScore,
            m.MatchDate,
            m.Location,
            StatusText = GetStatusText(m.Status)
        });

        return Ok(result);
    }

    // Queue'daki bir sonraki maçı getir
    [HttpGet("next")]
    public IActionResult GetNextMatch()
    {
        if (_matchQueue.Count() == 0)
        {
            RefreshMatchQueue();

            if (_matchQueue.Count() == 0)
            {
                return NotFound("Sırada bekleyen maç bulunamadı.");
            }
        }

        // Queue'dan bir sonraki maçı al ama çıkarma (peek)
        var nextMatch = _matchQueue.Peek();

        return Ok(new
        {
            nextMatch.Id,
            HomeTeamName = nextMatch.HomeTeamName,
            AwayTeamName = nextMatch.AwayTeamName,
            nextMatch.MatchDate,
            nextMatch.Location,
            StatusText = GetStatusText(nextMatch.Status),
            QueueCount = _matchQueue.Count()
        });
    }

    // Bir sonraki maçı queue'dan çıkar ve işle
    [HttpPost("process-next")]
    public IActionResult ProcessNextMatch()
    {
        if (_matchQueue.Count() == 0)
        {
            RefreshMatchQueue();

            if (_matchQueue.Count() == 0)
            {
                return NotFound("Sırada bekleyen maç bulunamadı.");
            }
        }

        // Queue'dan bir sonraki maçı çıkar (dequeue)
        var nextMatch = _matchQueue.Dequeue();

        // Burada maçı işleme mantığı eklenebilir
        // Örneğin maçın durumunu "Devam Ediyor" olarak güncelleyebilirsiniz
        nextMatch.Status = 1; // Devam Ediyor
        _context.SaveChanges();

        return Ok(new
        {
            Message = "Bir sonraki maç başlatıldı",
            Match = new
            {
                nextMatch.Id,
                HomeTeamName = nextMatch.HomeTeamName,
                AwayTeamName = nextMatch.AwayTeamName,
                nextMatch.MatchDate,
                nextMatch.Location,
                StatusText = GetStatusText(nextMatch.Status)
            },
            RemainingInQueue = _matchQueue.Count()
        });
    }

    // Admin paneli için tüm queue'yu getir
    [HttpGet("queue")]
    public IActionResult GetMatchQueue()
    {
        // Queue'yu yenile
        RefreshMatchQueue();

        // Queue'daki maçları listeye dönüştür
        var queuedMatches = _matchQueue.ToList().Select(m => new
        {
            m.Id,
            HomeTeamName = m.HomeTeamName,
            AwayTeamName = m.AwayTeamName,
            m.MatchDate,
            m.Location,
            StatusText = GetStatusText(m.Status)
        }).ToList();

        return Ok(new
        {
            QueueCount = queuedMatches.Count,
            Matches = queuedMatches
        });
    }

    // Maç durumunu metin olarak döndürür
    private string GetStatusText(int status)
    {
        return status switch
        {
            0 => "Planlandı",
            1 => "Devam Ediyor",
            2 => "Tamamlandı",
            3 => "İptal Edildi",
            _ => "Bilinmiyor"
        };
    }
}
