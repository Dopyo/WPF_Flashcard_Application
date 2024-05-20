## Chinese Flashcard Generator with Gemini

This WPF application leverages Google's Gemini Pro model to create Chinese flashcards. 

### Features

* **Random Word Selection:** Picks a random word from a provided HSK 1 word list.
* **Sentence Generation:** Uses Gemini to construct a Chinese sentence incorporating the chosen word.
* **Sentence Translation & Pinyin:**  Provides an English translation of the sentence and includes its pinyin representation.
* **Flashcard-like Interface:** Presents the word, sentence, and hides the translation for a flashcard-like learning experience.

### Prerequisites

* **NET 7:**  Ensure you have NET 7 installed on your system.
* **Visual Studio:**  This project is built using Visual Studio. You can download the free Community edition from [https://visualstudio.microsoft.com/](https://visualstudio.microsoft.com/).
* **Google Gemini API Key:** Obtain an API key for Google's Gemini Pro model from [https://developers.generativeai.google/](https://developers.generativeai.google/). Paste your key into the code as indicated in the `MainWindow.xaml.cs` file.

### Setup

1. **Clone the Repository:**  Clone this repository to your local machine
2. **API Key:** Open the `MainWindow.xaml.cs` file and replace `"Your API Key"` with your actual Gemini API key.

### How to Use

1. **Build and Run:** Build the project in Visual Studio.
2. **Generate Flashcard:** Click the "Run" button in the application to generate a new flashcard with a random HSK 1 word and a Chinese sentence using that word.
3. **Reveal Translation:** Click the "Check" button to reveal the English translation and pinyin of the Chinese sentence.

### Customization

* **HSK Levels:** Modify the code to utilize word lists from other HSK levels.
* **Word List Source:** Adapt the code to read from alternative word list formats or sources (e.g., databases, online APIs).
* **Sentence Complexity:**  Experiment with different prompts within the `CreateRequestBody` method to control the complexity or style of the generated sentences.

![](https://github.com/Dopyo/WPF_Flashcard_Application/blob/main/WPFGIF.gif)
