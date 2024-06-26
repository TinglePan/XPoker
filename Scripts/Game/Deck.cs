﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using Godot;
using XCardGame.Scripts.Cards;

using XCardGame.Scripts.Common.Constants;
using XCardGame.Scripts.Defs;
using XCardGame.Scripts.Defs.Def.Deck;

namespace XCardGame.Scripts.GameLogic;

public class Deck
{
    public ObservableCollection<BaseCard> CardList;

    public Deck(DeckDef def)
    {
        CardList = new ObservableCollection<BaseCard>();
        if (def.CardDefs != null)
        {
            foreach (var cardDef in def.CardDefs)
            {
                var card = CardFactory.CreateInstance(cardDef.ConcreteClassPath, cardDef);
                CardList.Add(card);
            }
        }
    }
}