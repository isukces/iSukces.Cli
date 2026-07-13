/*
using iSukces.Cli.Python;
using iSukces.Llm.DocumentCoverters.Abstract;
using iSukces.TextProcessing;

namespace iSukces.Cli.Punctuate;


/// <summary>
/// Serwis dzielący na zdania i przywracający interpunkcję w oparciu o kredor/punctuate-all
/// </summary>
/// <param name="resolver"></param>
public class PunctuateSentencesExtractor(IVenvCollectionResolver resolver) :
    ISentencesExtractor
{
    
    public async Task<SentencesExtractorResult> ExtractSentences(string rawText,
        IStateFilesHolder? stateHolder,
        CancellationToken cancellationToken)
    { 
        var venv = resolver.Get("punctuate");
        var q = new PunctExec(venv)
        {
            StateHolder = stateHolder
        };
        var output = await q.Process(rawText);
        return new SentencesExtractorResult(output);
    }
}
*/
