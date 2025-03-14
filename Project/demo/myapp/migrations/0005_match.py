# Generated by Django 4.2.20 on 2025-03-14 09:07

from django.db import migrations, models
import django.db.models.deletion


class Migration(migrations.Migration):

    dependencies = [
        ('myapp', '0004_remove_team_score'),
    ]

    operations = [
        migrations.CreateModel(
            name='Match',
            fields=[
                ('id', models.BigAutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('team1_score', models.IntegerField(default=0)),
                ('team2_score', models.IntegerField(default=0)),
                ('team1', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, related_name='away_matches', to='myapp.team')),
            ],
        ),
    ]
