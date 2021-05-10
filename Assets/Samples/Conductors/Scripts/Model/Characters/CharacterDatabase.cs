namespace Caliburn.Noesis.Samples.Conductors.Model.Characters
{
    /// <summary>The database containing information about all NPCs.</summary>
    /// <seealso cref="PropertyChangedBase" />
    public class CharacterDatabase : PropertyChangedBase
    {
        #region Constructors and Destructors

        /// <inheritdoc />
        public CharacterDatabase()
        {
            var eskel = new Character { DisplayName = "Eskel" };
            eskel.DescriptionParagraphs.AddRange(
                new[]
                    {
                        new DescriptionParagraph
                            {
                                Text =
                                    "Eskel was a witcher of the School of the Wolf taught by Master Vesemir at Kaer Morhen " +
                                    "and active in the 13th century. As the sorceress Triss Merigold noticed, Eskel emanated " +
                                    "a strong magic aura. Though less famous than his peer Geralt of Rivia, Eskel became " +
                                    "renowned in his own right as a professional, reliable witcher, and possessing a kind " +
                                    "and relaxed disposition.",
                                HasBeenRead = true,
                            },
                        new DescriptionParagraph
                            {
                                Text =
                                    "Eskel and the White Wolf were like brothers. Side by side they grew up, enjoyed pastime " +
                                    "activities and endured the Trials. A good friend of younger Lambert and Coën of Poviss, " +
                                    "Eskel saw Vesemir as both a mentor and a father figure. He returned to Kaer Morhen each " +
                                    "winter to swap tales and rejuvenate for The Path.",
                                HasBeenRead = true,
                            },
                        new DescriptionParagraph
                            {
                                Text =
                                    "Eskel was a witcher of the School of the Wolf taught by Master Vesemir at Kaer Morhen " +
                                    "and active in the 13th century. As the sorceress Triss Merigold noticed, Eskel emanated " +
                                    "a strong magic aura. Though less famous than his peer Geralt of Rivia, Eskel became " +
                                    "renowned in his own right as a professional, reliable witcher, and possessing a kind " +
                                    "and relaxed disposition.",
                                HasBeenRead = true,
                            },
                        new DescriptionParagraph
                            {
                                Text =
                                    "Eskel and the White Wolf were like brothers. Side by side they grew up, enjoyed pastime " +
                                    "activities and endured the Trials. A good friend of younger Lambert and Coën of Poviss, " +
                                    "Eskel saw Vesemir as both a mentor and a father figure. He returned to Kaer Morhen each " +
                                    "winter to swap tales and rejuvenate for The Path.",
                                HasBeenRead = true,
                            },
                        new DescriptionParagraph
                            {
                                Text =
                                    "Eskel was a witcher of the School of the Wolf taught by Master Vesemir at Kaer Morhen " +
                                    "and active in the 13th century. As the sorceress Triss Merigold noticed, Eskel emanated " +
                                    "a strong magic aura. Though less famous than his peer Geralt of Rivia, Eskel became " +
                                    "renowned in his own right as a professional, reliable witcher, and possessing a kind " +
                                    "and relaxed disposition.",
                                HasBeenRead = true,
                            },
                        new DescriptionParagraph
                            {
                                Text =
                                    "Eskel and the White Wolf were like brothers. Side by side they grew up, enjoyed pastime " +
                                    "activities and endured the Trials. A good friend of younger Lambert and Coën of Poviss, " +
                                    "Eskel saw Vesemir as both a mentor and a father figure. He returned to Kaer Morhen each " +
                                    "winter to swap tales and rejuvenate for The Path.",
                                HasBeenRead = true,
                            },
                        new DescriptionParagraph
                            {
                                Text =
                                    "Eskel was a witcher of the School of the Wolf taught by Master Vesemir at Kaer Morhen " +
                                    "and active in the 13th century. As the sorceress Triss Merigold noticed, Eskel emanated " +
                                    "a strong magic aura. Though less famous than his peer Geralt of Rivia, Eskel became " +
                                    "renowned in his own right as a professional, reliable witcher, and possessing a kind " +
                                    "and relaxed disposition.",
                                HasBeenRead = false,
                            },
                        new DescriptionParagraph
                            {
                                Text =
                                    "Eskel and the White Wolf were like brothers. Side by side they grew up, enjoyed pastime " +
                                    "activities and endured the Trials. A good friend of younger Lambert and Coën of Poviss, " +
                                    "Eskel saw Vesemir as both a mentor and a father figure. He returned to Kaer Morhen each " +
                                    "winter to swap tales and rejuvenate for The Path.",
                                HasBeenRead = false,
                            },
                    });

            Characters.Add(eskel);

            var lambert = new Character { DisplayName = "Lambert" };
            lambert.DescriptionParagraphs.AddRange(
                new[]
                    {
                        new DescriptionParagraph
                            {
                                Text =
                                    "Lambert was one of the younger witchers from Kaer Morhen. Known for his biting tongue, " +
                                    "he was often rude in conversation. He was particularly rude towards Triss Merigold, " +
                                    "addressing her only by her last name, which irritated the sorceress greatly.",
                                HasBeenRead = true,
                            },
                        new DescriptionParagraph
                            {
                                Text =
                                    "He does not have much use for politics, a trait common to most witchers due to their code of " +
                                    "neutrality. He helped train Ciri in the art of combat. He was described as being at the same " +
                                    "age as Coën. He is one of the last witchers to be trained within the walls of Kaer Morhen.",
                                HasBeenRead = true,
                            },
                        new DescriptionParagraph
                            {
                                Text =
                                    "Lambert was one of the younger witchers from Kaer Morhen. Known for his biting tongue, " +
                                    "he was often rude in conversation. He was particularly rude towards Triss Merigold, " +
                                    "addressing her only by her last name, which irritated the sorceress greatly.",
                                HasBeenRead = true,
                            },
                        new DescriptionParagraph
                            {
                                Text =
                                    "He does not have much use for politics, a trait common to most witchers due to their code of " +
                                    "neutrality. He helped train Ciri in the art of combat. He was described as being at the same " +
                                    "age as Coën. He is one of the last witchers to be trained within the walls of Kaer Morhen.",
                                HasBeenRead = true,
                            },
                        new DescriptionParagraph
                            {
                                Text =
                                    "Lambert was one of the younger witchers from Kaer Morhen. Known for his biting tongue, " +
                                    "he was often rude in conversation. He was particularly rude towards Triss Merigold, " +
                                    "addressing her only by her last name, which irritated the sorceress greatly.",
                                HasBeenRead = true,
                            },
                        new DescriptionParagraph
                            {
                                Text =
                                    "He does not have much use for politics, a trait common to most witchers due to their code of " +
                                    "neutrality. He helped train Ciri in the art of combat. He was described as being at the same " +
                                    "age as Coën. He is one of the last witchers to be trained within the walls of Kaer Morhen.",
                                HasBeenRead = false,
                            },
                        new DescriptionParagraph
                            {
                                Text =
                                    "Lambert was one of the younger witchers from Kaer Morhen. Known for his biting tongue, " +
                                    "he was often rude in conversation. He was particularly rude towards Triss Merigold, " +
                                    "addressing her only by her last name, which irritated the sorceress greatly.",
                                HasBeenRead = false,
                            },
                        new DescriptionParagraph
                            {
                                Text =
                                    "He does not have much use for politics, a trait common to most witchers due to their code of " +
                                    "neutrality. He helped train Ciri in the art of combat. He was described as being at the same " +
                                    "age as Coën. He is one of the last witchers to be trained within the walls of Kaer Morhen.",
                                HasBeenRead = false,
                            },
                    });

            Characters.Add(lambert);

            var vesemir = new Character { DisplayName = "Vesemir" };
            vesemir.DescriptionParagraphs.AddRange(
                new[]
                    {
                        new DescriptionParagraph
                            {
                                Text =
                                    "Vesemir was the oldest and most experienced witcher at Kaer Morhen in the 13th century and acted " +
                                    "as a father figure to Geralt and the other witchers. Like many of the other witchers, he spent " +
                                    "each winter in the fortress and set out on the path when spring arrived.",
                                HasBeenRead = true,
                            },
                        new DescriptionParagraph
                            {
                                Text =
                                    "He was one of the few members of the School of the Wolf to survive the assault on Kaer Morhen. " +
                                    "By the 1260s, he was the sole old witcher remaining; however, as he was only a fencing instructor, " +
                                    "he didn't possess the knowledge necessary to create new mutagens in order to mutate more disciples " +
                                    "into witchers.",
                                HasBeenRead = true,
                            },
                        new DescriptionParagraph
                            {
                                Text =
                                    "Vesemir was the oldest and most experienced witcher at Kaer Morhen in the 13th century and acted " +
                                    "as a father figure to Geralt and the other witchers. Like many of the other witchers, he spent " +
                                    "each winter in the fortress and set out on the path when spring arrived.",
                                HasBeenRead = true,
                            },
                        new DescriptionParagraph
                            {
                                Text =
                                    "He was one of the few members of the School of the Wolf to survive the assault on Kaer Morhen. " +
                                    "By the 1260s, he was the sole old witcher remaining; however, as he was only a fencing instructor, " +
                                    "he didn't possess the knowledge necessary to create new mutagens in order to mutate more disciples " +
                                    "into witchers.",
                                HasBeenRead = true,
                            },
                        new DescriptionParagraph
                            {
                                Text =
                                    "Vesemir was the oldest and most experienced witcher at Kaer Morhen in the 13th century and acted " +
                                    "as a father figure to Geralt and the other witchers. Like many of the other witchers, he spent " +
                                    "each winter in the fortress and set out on the path when spring arrived.",
                                HasBeenRead = false,
                            },
                        new DescriptionParagraph
                            {
                                Text =
                                    "He was one of the few members of the School of the Wolf to survive the assault on Kaer Morhen. " +
                                    "By the 1260s, he was the sole old witcher remaining; however, as he was only a fencing instructor, " +
                                    "he didn't possess the knowledge necessary to create new mutagens in order to mutate more disciples " +
                                    "into witchers.",
                                HasBeenRead = false,
                            },
                        new DescriptionParagraph
                            {
                                Text =
                                    "Vesemir was the oldest and most experienced witcher at Kaer Morhen in the 13th century and acted " +
                                    "as a father figure to Geralt and the other witchers. Like many of the other witchers, he spent " +
                                    "each winter in the fortress and set out on the path when spring arrived.",
                                HasBeenRead = false,
                            },
                        new DescriptionParagraph
                            {
                                Text =
                                    "He was one of the few members of the School of the Wolf to survive the assault on Kaer Morhen. " +
                                    "By the 1260s, he was the sole old witcher remaining; however, as he was only a fencing instructor, " +
                                    "he didn't possess the knowledge necessary to create new mutagens in order to mutate more disciples " +
                                    "into witchers.",
                                HasBeenRead = false,
                            },
                    });

            Characters.Add(vesemir);
        }

        #endregion

        #region Public Properties

        /// <summary>Gets a list of all NPCs.</summary>
        /// <value>A list of all NPCs.</value>
        public IBindableCollection<Character> Characters { get; } =
            new BindableCollection<Character>();

        #endregion
    }
}