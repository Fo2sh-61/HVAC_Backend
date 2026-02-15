function [best_params, best_rmse, convergence_history] = Differential_evolution( ...
    V_meas, I_meas, Vt_ref, N_diodes, lb, ub, opt_params)
%DIFFERENTIAL_EVOLUTION Estimate PV parameters using DE/rand/1/bin.
%   [best_params, best_rmse, convergence_history] = Differential_evolution( ...
%       V_meas, I_meas, Vt_ref, N_diodes, lb, ub, opt_params)
%
% Inputs
%   V_meas     : Measured voltage vector
%   I_meas     : Measured current vector
%   Vt_ref     : Thermal voltage at reference temperature
%   N_diodes   : Number of diodes in model (1, 2, or 3)
%   lb         : Lower bounds (1 x D)
%   ub         : Upper bounds (1 x D)
%   opt_params : Struct with DE options (all optional)
%       .NP        - Population size (default: 10 * D)
%       .max_iter  - Generations (default: 150)
%       .F         - Scaling factor, mutation weight (default: 0.85)
%       .CR        - Crossover rate (default: 0.90)
%       .strategy  - Must be 'DE/rand/1/bin' (default)
%       .verbose   - Print progress (default: true)
%
% Outputs
%   best_params          : Estimated parameter vector
%   best_rmse            : Best RMSE value found
%   convergence_history  : Best RMSE per iteration

    if nargin < 7 || isempty(opt_params)
        opt_params = struct();
    end

    V_meas = V_meas(:);
    I_meas = I_meas(:);
    lb = lb(:).';
    ub = ub(:).';

    if numel(V_meas) ~= numel(I_meas)
        error('V_meas and I_meas must have the same number of elements.');
    end
    if numel(lb) ~= numel(ub)
        error('lb and ub must have the same number of elements.');
    end
    if any(ub <= lb)
        error('Each upper bound must be strictly greater than lower bound.');
    end

    D = numel(lb);

    NP = round(read_option(opt_params, {'NP', 'de_pop_size', 'population_size', 'pop_size'}, 10 * D));
    max_iter = round(read_option(opt_params, {'max_iter', 'de_iterations', 'iterations'}, 150));
    F = read_option(opt_params, {'F', 'de_F', 'mutation_factor'}, 0.85);
    CR = read_option(opt_params, {'CR', 'de_CR', 'crossover_rate'}, 0.90);
    strategy = read_option(opt_params, {'strategy', 'de_strategy'}, 'DE/rand/1/bin');
    verbose = logical(read_option(opt_params, {'verbose', 'display'}, true));
    print_every = round(read_option(opt_params, {'display_every', 'print_every'}, 25));

    NP = max(4, NP);
    max_iter = max(1, max_iter);
    F = min(max(F, eps), 2.0);
    CR = min(max(CR, 0.0), 1.0);
    print_every = max(1, print_every);

    if ~strcmpi(strategy, 'DE/rand/1/bin')
        warning('Differential_evolution:Strategy', ...
            'Only DE/rand/1/bin is implemented. Falling back to DE/rand/1/bin.');
    end

    span = ub - lb;
    population = lb + rand(NP, D) .* span;
    fitness = inf(NP, 1);

    for i = 1:NP
        fitness(i) = rmse_objective(population(i, :), V_meas, I_meas, Vt_ref, N_diodes);
    end

    [best_rmse, idx_best] = min(fitness);
    best_params = population(idx_best, :);
    convergence_history = zeros(max_iter, 1);

    for g = 1:max_iter
        for i = 1:NP
            candidates = 1:NP;
            candidates(i) = [];
            r = candidates(randperm(NP - 1, 3));

            mutant = population(r(1), :) + F * (population(r(2), :) - population(r(3), :));
            mutant = max(lb, min(ub, mutant));

            j_rand = randi(D);
            cross_mask = rand(1, D) <= CR;
            cross_mask(j_rand) = true;

            trial = population(i, :);
            trial(cross_mask) = mutant(cross_mask);
            trial = max(lb, min(ub, trial));

            trial_fit = rmse_objective(trial, V_meas, I_meas, Vt_ref, N_diodes);

            if trial_fit <= fitness(i)
                population(i, :) = trial;
                fitness(i) = trial_fit;

                if trial_fit < best_rmse
                    best_rmse = trial_fit;
                    best_params = trial;
                end
            end
        end

        convergence_history(g) = best_rmse;

        if verbose && (g == 1 || g == max_iter || mod(g, print_every) == 0)
            fprintf('DE iter %3d/%3d -> best RMSE = %.6e A\n', g, max_iter, best_rmse);
        end
    end
end

function rmse = rmse_objective(params, V_meas, I_meas, Vt_ref, N_diodes)
    I_model = zeros(size(V_meas));

    for k = 1:numel(V_meas)
        I_model(k) = multi_diode_model(V_meas(k), params, Vt_ref, N_diodes);
    end

    if any(~isfinite(I_model))
        rmse = realmax;
        return;
    end

    err = I_meas - I_model;
    rmse = sqrt(mean(err .^ 2));

    if ~isfinite(rmse)
        rmse = realmax;
    end
end

function value = read_option(s, names, default_value)
    value = default_value;
    for i = 1:numel(names)
        f = names{i};
        if isfield(s, f) && ~isempty(s.(f))
            value = s.(f);
            return;
        end
    end
end
