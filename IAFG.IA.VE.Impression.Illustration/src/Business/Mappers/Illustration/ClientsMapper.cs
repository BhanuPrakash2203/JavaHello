using System;
using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.Illustration;
using ProjectionData = IAFG.IA.VI.Projection.Data;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Types;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.Illustration
{
    public class ClientsMapper : IClientsMapper
    {
        public List<Client> MapClients(IEnumerable<DonneesClient> clients, ProjectionData.Projection projection)
        {
            var result = new List<Client>();
            MapperIndividus(projection?.Contract?.Individuals, CreerJoint(projection?.Contract), result);
            MapperDataClients(projection?.DataClients?.List, result);
            MapperClients(clients, result);
            return result;
        }

        private List<ProjectionData.Contract.Coverage.Joint> CreerJoint(ProjectionData.Contract.Contract contract)
        {
            var listjoints = new List<ProjectionData.Contract.Coverage.Joint>();
            if (contract?.Insured == null) return listjoints;
            foreach (var insured in contract.Insured)
            {
                foreach (var coverage in insured.Coverages)
                {
                    var joints = coverage.Insured.Joints?.Where(joint => joint != null);
                    if (joints != null)
                    {
                        listjoints.AddRange(joints);
                    }
                }
            }

            return listjoints;
        }

        public void MapperDataClients(IEnumerable<ProjectionData.DataClient.DataClient> donneesClients,
            List<Client> clients)
        {
            if (donneesClients == null)
            {
                return;
            }

            foreach (var item in donneesClients)
            {
                var client = clients.FirstOrDefault(x => x.ReferenceExterneId == item.Identifier.Id);
                if (client == null)
                {
                    clients.Add(MapperDataClient(item));
                }
                else
                {
                    MapperDataClient(client, item);
                }
            }

        }

        private Client MapperDataClient(ProjectionData.DataClient.DataClient dataClient)
        {
            var result = new Client
            {
                ReferenceExterneId = dataClient.Identifier.Id,
            };

            return MapperDataClient(result, dataClient);
        }

        private Client MapperDataClient(Client client, ProjectionData.DataClient.DataClient dataClient)
        {
            client.AgeAssurance = dataClient.InsuranceAge;
            client.Nom = dataClient.LastName;
            client.Prenom = dataClient.FirstName;
            client.Initiale = dataClient.Initial;
            client.Sexe = dataClient.Sex.ConvertirSex();
            return client;
        }

        private void MapperClients(IEnumerable<DonneesClient> donneesClients, ICollection<Client> clients)
        {
            if (donneesClients == null)
            {
                return;
            }

            foreach (var item in donneesClients)
            {
                var client = clients.FirstOrDefault(x => x.ReferenceExterneId == item.Id);
                if (client == null)
                {
                    clients.Add(MapperClient(item));
                }
                else
                {
                    MapperClient(client, item);
                }
            }
        }

        private Client MapperClient(DonneesClient donneesClient)
        {
            var result = new Client
            {
                ReferenceExterneId = donneesClient.Id,
            };

            return MapperClient(result, donneesClient);
        }

        private Client MapperClient(Client client, DonneesClient donneesClient)
        {
            client.Nom = donneesClient.Nom;
            client.Prenom = donneesClient.Prenom;
            client.Initiale = donneesClient.Initiale;
            if (donneesClient.AgeAssurance.HasValue) client.AgeAssurance = donneesClient.AgeAssurance;
            if (donneesClient.Sexe.HasValue) client.Sexe = donneesClient.Sexe.Value;
            if (donneesClient.StatutFumeur.HasValue) client.StatutFumeur = donneesClient.StatutFumeur.Value;
            return client;
        }

        public void MapperIndividus(IEnumerable<ProjectionData.Contract.Individual> individuals, 
            List<ProjectionData.Contract.Coverage.Joint> joints, List<Client> clients)
        {
            if (individuals != null)
            {
                clients.AddRange(individuals.Select(c => MapperIndividu(c, joints)));
            }
        }

        private Client MapperIndividu(ProjectionData.Contract.Individual individu,
            List<ProjectionData.Contract.Coverage.Joint> joints)
        {
            var result = new Client
            {
                Sexe = individu.Sex.ConvertirSex(),
                DateNaissance = individu.Birthdate.HasValue && individu.Birthdate.Value > DateTime.MinValue
                    ? individu.Birthdate
                    : null,
                EstContractant = individu.IsApplicant,
                SequenceIndividu = individu.SequenceNumber,
                ReferenceExterneId = individu.Identifier.Id,
            };

            if (joints.Any())
            {
                result.IsNotAssurable =
                    joints.FirstOrDefault(x => x.InsuredIndividual?.Individual?.Id == individu.Identifier.Id)
                        ?.IsNotInsurable;
            }

            return result;
        }
    }
}
