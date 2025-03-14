# Generated by Django 4.2.20 on 2025-03-14 09:45

from django.db import migrations, models
import django.db.models.deletion


class Migration(migrations.Migration):

    dependencies = [
        ('myapp', '0008_alter_team_draw_alter_team_lose_alter_team_win'),
    ]

    operations = [
        migrations.AlterField(
            model_name='match',
            name='team1',
            field=models.ForeignKey(default=None, on_delete=django.db.models.deletion.CASCADE, related_name='home_matches', to='myapp.team'),
        ),
        migrations.AlterField(
            model_name='match',
            name='team2',
            field=models.ForeignKey(default=None, on_delete=django.db.models.deletion.CASCADE, related_name='away_matches', to='myapp.team'),
        ),
    ]
