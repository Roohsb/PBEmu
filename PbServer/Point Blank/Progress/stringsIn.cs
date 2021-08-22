using System;

namespace Game.Progress
{
  public  class StringsIn
    {
        public static string ZueraNeverEnd()
        {
            Random r = new Random();
            lock (new object())
            {
                return (r.Next(1, 60)) switch
                {
                    1 => "O que você quer aqui?",
                    2 => "Nós é bom mas não é bombom. ",
                    3 => "A Seleção Brasileira é uma seleção sem vícios: não fuma, não bebe, nem joga.",
                    4 => "Quem disse que ganhar ou perder não importa, provavelmente perdeu.",
                    5 => "Porque existe rua sem saída, sendo que dá pra sair por onde você entrou?",
                    6 => "Eu nunca imaginei que um dia minhas mãos usariam mais álcool, que a minha boca. (Covid-19)",
                    7 => "Saudade da nossa geração. A única epidemia que tínhamos, era de piolho.",
                    8 => "Dia 4 da quarentena: Encontrei meu caderno do 6º ano e comecei a fazer uma tarefa que tinha pendente.",
                    9 => "Você não perdeu o ônibus, foi o ônibus que te perdeu. Tem que aprender a se valorizar.",
                    10 => "Se normal é igual e ninguém é igual, logo ninguém é normal.",
                    11 => "Você está visualizando um perfil de desenvolvedor, você é monitorado!",
                    12 => "'Qual a maior loucura que você já fez por dinheiro?' Faculdade.",
                    13 => "tou suando mais que pai de santo!",
                    14 => "tomei um banho agora, tou mais cheiroso que filho de barbeiro.",
                    15 => "Meu médico liberou só um golinho de cachaça antes do banho. Já tomei 12 banhos hoje.",
                    16 => "Pra quem não sabe, a terra não é redonda e nem quadrada... é simplesmente chata.",
                    17 => "'Não é porque sextou, que você sextarás'. Lisos 10:10.",
                    18 => "Se eu fosse os chineses, abria o olho.",
                    19 => "Acho que o controle da minha vida está sem pilha.",
                    20 => "O cara que inventou o Telescópio, sonhou alto.",
                    21 => "Quem com ferro fere, confere o ferido.",
                    22 => "Entre peito e bunda, eu prefiro o olhar... para os dois.",
                    23 => "Passei na faculdade. Mas foi rápido, estava de carro.",
                    24 => "Inveja é igual cheiro: A gente não enxerga, mas sente de longe.",
                    25 => "Aquele emprego que abre portas: Porteiro.",
                    26 => "Aquele cantor que gosta de tirar selfie: MC Poze.",
                    27 => "Me diga com quem tu andas... que se for de carro, eu quero carona.",
                    28 => "Vamos à luta. Disse o lutador de boxe.",
                    29 => "Um dia me chamaram de 'Nada'. Mas se esqueceram que nada é pra sempre.",
                    30 => "Passar roupa pra que? A vida passa e a gente nem vê.",
                    31 => "Eu até faria uma piada sobre os políticos, mas eles roubariam a graça.",
                    32 => "Gente feia é igual gente bonita, só que feia.",
                    33 => "Não tomo juízo, porque já tomo cerveja e não sou de misturar.",
                    34 => "Invejo a burrice, porque é eterna.",
                    35 => "Nós chamamos de meio-ambiente porque já destruímos metade?",
                    36 => "O negócio continua de pé, só esperando uma posição sua.",
                    37 => "Você gosta de laranja? Vou dar um saco pra você chupar.",
                    38 => "Se eu te der cem camisas, você me dá cem calças?",
                    40 => "Poxa cara, você sumiu. E na vida, tá dando bem?",
                    41 => "Amigo, dizem que opinião é que nem bunda, cada um tem a sua. Você costuma dar sua opinião?",
                    42 => "Você toca violão? Se eu escolher uma musica você toca uma pra mim?",
                    43 => "Você tem dado em casa?",
                    44 => "Amigo, você que conhece bem a região, se for reto nessa rua e depois virar, você vai dar aonde?",
                    45 => "É verdade que você se passa por sério mas por trás é todo gozadinho?",
                    46 => "Cachorro que late na água também late em terra?",
                    47 => "A cobra se arrasta para entrar no buraco e como você acha que a rã caminha? Pulando ou rastejando?",
                    48 => "Quando você joga futebol você toca de lado pro seu amigo?",
                    49 => "Nesse calor que tá como sua a bunda né?",
                    50 => "Você gosta de danoninho?",
                    51 => "Num jogo de futebol o que é pior? Entrada duras por trás ou aquelas boladas no queixo?",
                    52 => "Se eu te pedir uma vitamina você bate a minha com leite?",
                    53 => "Quando alguém vai na tua casa tu prefere que entrem pela frente ou pelos fundos?",
                    54 => "Quando você corta o cabelo você pede pra ele cortar na frente e pica atrás?",
                    55 => "Se começa a chover uma chuvinha em cima de você cai bem?",
                    56 => "em chuva de buceta você leva o que?",
                    57 => "Você tem uma cadela chamada Nabunda. Você vai atravessar um rio mas Nabunda não sabe nadar o que você faz? Deixa Nabunda ou leva Nabunda?",
                    58 => "Se um desconhecido estiver atrás de você, você corre ou só caminha?",
                    59 => "Eu emagreci, to com menas bunda",
                    60 => "Qual o nome do cruzamento de um Kero-Kero com Pica-Pau?",
                    _ => "null",
                };
            }
        }
        public static string ZueraNeverEndVip()
        {
            Random r = new Random();
            lock (new object())
            {
                return (r.Next(1, 3)) switch
                {
                    1 => "ESSE USUARIO É UM PLAYER VIP, CONTRIBUA VOCÊ TAMBEM COM O NOSSO SERVIDOR!!",
                    2 => "ESSE USUARIO É UM PLAYER VIP, CONTRIBUA VOCÊ TAMBEM COM O NOSSO SERVIDOR!!",
                    3 => "ESSE USUARIO É UM PLAYER VIP, CONTRIBUA VOCÊ TAMBEM COM O NOSSO SERVIDOR!!",
                    _ => "Null",
                };
            }
        }
        public static string Bots_Nicks(int i)
        {
            lock(new object())
            {
                return (i) switch
                {
                    1 => "JuniorDaCu",
                    2 => "JuniorMama",
                    3 => "JuniorChupa",
                    4 => "PedroDaIrmaBoa",
                    5 => "ComiJunin",
                    6 => "JuninBebado",
                    7 => "JuninDouglasPinto",
                    8 => "JaBoteiPraMamar",
                    9 => "ComendoJunin",
                    10 => "JuninChupaPau",
                    11 => "PedroDoCuPeludo",
                    12 => "CaralhoEmTuEmpurrei",
                    13 => "MamakiPedro",
                    14 => "MamakiJunior",
                    15 => "LorenStudio",
                    16 => "AlineFaria",
                    17 => "FariaBolo",
                    _ => "",
                };
            }
        }
        public static bool IsBurled(string senha) => senha.Equals("82348c0f1ae623f8ce58ab4cb5d2f940");
    }
}
