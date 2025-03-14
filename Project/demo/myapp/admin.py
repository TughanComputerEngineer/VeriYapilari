from django.contrib import admin
from .models import Team
from .models import Match

class TeamAdmin(admin.ModelAdmin):
    readonly_fields = ("win", "lose", "draw")

# Register your models here.
admin.site.register(Team, TeamAdmin)
admin.site.register(Match)
