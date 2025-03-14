from django.db import models

class Team(models.Model):
    name = models.CharField(max_length=255)

    win = models.IntegerField(default = 0, editable = False)
    lose = models.IntegerField(default = 0, editable = False)
    draw = models.IntegerField(default = 0, editable = False)

    @property
    def score(self):
        return self.win * 3 + self.draw

    def __str__(self):
        return f"{self.name} ({self.score})"
    
class Match(models.Model):
    team1 = models.ForeignKey(Team, on_delete=models.CASCADE, related_name = "home_matches", default = 0)
    team2 = models.ForeignKey(Team, on_delete=models.CASCADE, related_name = "away_matches", default = 1)

    team1_score = models.IntegerField(default = 0)
    team2_score = models.IntegerField(default = 0)

    def play_matches(self, team1_score, team2_score):
        if team1_score > team2_score:
            self.team1.win += 1
            self.team2.lose += 1
        elif team1_score < team2_score:
            self.team1.lose += 1
            self.team2.win += 1
        else:
            self.team1.draw += 1
            self.team2.draw += 1
        
        self.team1.save()
        self.team2.save()

    def save(self):
        self.play_matches(self.team1_score, self.team2_score)
        super().save()