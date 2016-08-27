using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    abstract class Job
    {

        protected float techProgress;

        abstract internal void Tick();
    }

    class Farm : Job
    {
        protected float stock;
        float efficiency = 1;
        float production { get { return People.PeopleAt(People.Community.farm) * efficiency * (1 - People.unrest[People.Community.farm]); } }
        internal override void Tick()
        {
            stock += production;
            if (stock > production * 2)
            {
                stock = production * 2;
                God.TheOne.Console("Food stock overflow. Stop wasting food!");
            }
        }
    }

    class Quarry : Job
    {
        protected float stock;
        float efficiency = 0.05f;   // you need 20 people to make 1 stone
        float production { get { return People.PeopleAt(People.Community.quarry) * efficiency * (1 - People.unrest[People.Community.quarry]); } }
        internal override void Tick()
        {
            stock += production;
        }

        internal int HasStones()
        {
            return (int)stock;
        }
    }

    class Road : Job
    {
        int teamsize = 10;
        int transportTime = 5;      // time to bring a stone to the docks
        List<Team> teamsOnTheWay = new List<Team>();

        internal override void Tick()
        {
            int availablePeople = People.PeopleAt(People.Community.transportRoad) - teamsOnTheWay.Sum(t => t.people);
            if (availablePeople < 0) God.TheOne.Console("Road has negative people!");
            int availableStones = God.TheOne.quarry.HasStones();
            int stonesToBeMoved = Math.Max(availablePeople / teamsize, availableStones);
            if (stonesToBeMoved > 0)
            {
                teamsOnTheWay.Add(new Team(stonesToBeMoved * teamsize, stonesToBeMoved));
            }
            for (int i = teamsOnTheWay.Count - 1; i >= 0; i--)
            {
                teamsOnTheWay[i].Time++;
                if(teamsOnTheWay[i].Time > transportTime)
                {
                    God.TheOne.river.newStonesArrive(teamsOnTheWay[i].stones);
                    teamsOnTheWay.RemoveAt(i);
                }
            }

        }

        class Team
        {
            readonly public int people;
            readonly public int stones;
            public int Time;
            public Team(int people, int stones) { this.people = people; this.stones = stones; Time = 0; }
        }
    }

    class River : Job
    {
        int dockStock;
        internal override void Tick()
        {
            throw new NotImplementedException();
        }

        internal void newStonesArrive(int stones) { dockStock += stones; }
    }

    class Construction : Job
    {
        internal override void Tick()
        {
            throw new NotImplementedException();
        }
    }

    class Military : Job
    {
        internal override void Tick()
        {
            throw new NotImplementedException();
        }
    }
}
