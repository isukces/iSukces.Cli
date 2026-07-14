using iSukces.CliTools;

namespace iSukces.Cli.Python;

/// <summary>
/// Python environment areas affected by a diagnostic problem.
/// </summary>
[Flags]
public enum ProblemAffected
{
    /// <summary>
    /// No affected environment area.
    /// </summary>
    None = 0,

    /// <summary>
    /// Python process execution.
    /// </summary>
    PythonExecution = 1,

    /// <summary>
    /// PyTorch package installation.
    /// </summary>
    PyTorchInstallation = 2,

    /// <summary>
    /// PyTorch version detection.
    /// </summary>
    PyTorchVersionDetection = 4,

    /// <summary>
    /// CUDA Toolkit installation.
    /// </summary>
    CudaToolkit = 8,

    /// <summary>
    /// CUDA version detection.
    /// </summary>
    CudaVersionDetection = 16
}

/// <summary>
/// Diagnostic analyzer for Python environment problems.
/// </summary>
/// <param name="value">Collected environment information to analyze.</param>
public sealed class PythonEnvironmentProblemFinder(PythonEnvironmentInfo value)
{
    private static bool IsError<T>(Result<T> result)
    {
        var isOk = result.IsSuccess &&
                   string.IsNullOrWhiteSpace(result.CliResult?.Error);
        return !isOk;
    }


    private bool Check01_IsTorchInstalledProperly()
    {
        var q = value.TorchVersion;
        if (q.IsSuccess)
            return true;

        var p1 = new PythonEnvironmentProblem
        {
            Message = "Nie udało się odczytać wersji PyTorch.",
            Description =
                """
                Wynik polecenia nie zawiera poprawnej informacji o wersji biblioteki PyTorch. 
                Instalacja może być uszkodzona lub środowisko Python jest niepoprawnie skonfigurowane.
                """,
            Affected = ProblemAffected.PyTorchInstallation | ProblemAffected.PyTorchVersionDetection,
            Remedy = """
                     Sprawdź, czy PyTorch jest poprawnie zainstalowany.
                     W razie potrzeby przeinstaluj bibliotekę w aktywnym środowisku Python.
                     """
        };
        list.Add(p1);
        return false;
    }

    private bool Check02_IsCudaAvailable()
    {
        var q = value.IsTorchCudaAvailable;
        if (IsError(q))
        {
            var p1 = new PythonEnvironmentProblem
            {
                Message = "Brak dostępnej akceleracji GPU (CUDA).",
                Description = """
                              Środowisko Python działa poprawnie, ale nie wykryto obsługi GPU. 
                              Modele AI będą uruchamiane wyłącznie na procesorze (CPU), co może znacząco obniżyć wydajność.
                              """,
                Affected = ProblemAffected.CudaToolkit,
                Remedy = """
                         Zainstaluj sterowniki GPU oraz zgodną wersję CUDA.
                         Upewnij się, że zainstalowana wersja PyTorch obsługuje CUDA.
                         """
            };
            list.Add(p1);
            return false;
        }

        if (q.Value)
            return true;
        var p2 = new PythonEnvironmentProblem
        {
            Message = "Błąd podczas uruchamiania PyTorch.",
            Description = "Wystąpił błąd przy próbie sprawdzenia dostępności GPU. " +
                          "Środowisko Python lub instalacja PyTorch może być niepoprawna.",
            Affected = ProblemAffected.PyTorchInstallation | ProblemAffected.PythonExecution,
            Remedy = """
                     Sprawdź, czy Python działa poprawnie oraz czy PyTorch jest zainstalowany i zgodny z wersją systemu. 
                     W razie potrzeby przeinstaluj środowisko.
                     """
        };
        list.Add(p2);
        return false;
    }

    private TorchCudaVersion? Check03_VerifyPyTorch()
    {
        var v = value.TorchCudaVersion;
        if (v.IsSuccess && v.Value is not null)
            return v.Value;

        var problem = new PythonEnvironmentProblem
        {
            Message = "Nie udało się odczytać wersji CUDA.",
            Description = "Akceleracja GPU jest dostępna, ale system nie zwraca informacji o wersji CUDA. Środowisko może być niekompletne lub niespójne.",
            Affected = ProblemAffected.CudaToolkit | ProblemAffected.CudaVersionDetection,
            Remedy = "Sprawdź instalację CUDA oraz zgodność wersji PyTorch z używaną kartą graficzną. W razie potrzeby przeinstaluj sterowniki lub PyTorch."
        };

        list.Add(problem);
        return null;
    }


    private void Check04_CheckInstalledVersions(TorchCudaVersion torchCudaVersion)
    {
        var iv = value.InstalledVersions.Value ?? [];
        if (iv.Count == 0)
        {
            var tmp = new PythonEnvironmentProblem
            {
                Message = "CUDA Toolkit nie jest zainstalowany w systemie.",
                Description = $"""
                               Funkcje wymagające kompilacji kerneli GPU (np. Triton) będą działać w trybie wolniejszym (CPU fallback).
                               Aby włączyć pełną akcelerację GPU, zainstaluj CUDA Toolkit w wersji zgodnej z używaną wersją PyTorch ({torchCudaVersion.Value}).
                               """,
                Affected = ProblemAffected.CudaToolkit,
                Remedy = $"""
                          1. Pobierz i zainstaluj CUDA Toolkit w wersji {torchCudaVersion.Value}.
                          2. Po instalacji uruchom ponownie system.
                          3. Zweryfikuj poprawność instalacji ponownie w aplikacji.
                          """
            };
            list.Add(tmp);
            return;
        }

        var found = iv.FirstOrDefault(a => a.Value == torchCudaVersion.Value);
        if (found is not null)
            return;

        var problem = new PythonEnvironmentProblem
        {
            Message = "Niezgodność wersji CUDA Toolkit.",
            Description = """
                          Wykryto niezgodność między wersją CUDA używaną przez PyTorch
                          a wersjami CUDA Toolkit zainstalowanymi w systemie.
                          Może to powodować problemy z działaniem akceleracji GPU.
                          """,
            Affected = ProblemAffected.CudaToolkit,
            Remedy = $"""
                      Możliwe rozwiązania:
                      1. Zainstaluj CUDA Toolkit w wersji {torchCudaVersion.Value}, aby dopasować ją do wersji używanej przez PyTorch.
                      2. Zainstaluj wersję PyTorch zgodną z aktualnie posiadanym CUDA Toolkit.
                      3. Jeśli właściwa wersja CUDA jest już zainstalowana, sprawdź zmienne środowiskowe (PATH, CUDA_PATH), aby wskazywały poprawną lokalizację.
                      Rekomendowane rozwiązanie:
                      Zainstaluj CUDA Toolkit w wersji dokładnie {torchCudaVersion.Value}.
                      """
        };

        list.Add(problem);
    }

    /// <summary>
    /// Detected problems for the analyzed Python environment.
    /// </summary>
    /// <returns>Detected environment problems.</returns>
    public IReadOnlyList<PythonEnvironmentProblem> GetProblems()
    {
        list.Clear();
        Update();
        return list;
    }

    private void Update()
    {
        if (!Check01_IsTorchInstalledProperly()) return;
        if (!Check02_IsCudaAvailable()) return;
        var ver1 = Check03_VerifyPyTorch();
        if (ver1 is null) return;
        Check04_CheckInstalledVersions(ver1);
    }

    private readonly List<PythonEnvironmentProblem> list = [];
}
